using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Lean.Touch
{
	/// <summary>This component allows you to detect </summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanShapeDetector")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Shape Detector")]
	public class LeanShapeDetector : MonoBehaviour
	{
		[System.Serializable] public class LeanFingerEvent : UnityEvent<LeanFinger> {}

		/// <summary>This stores data about a finger that's currently tracing the shape.</summary>
		[System.Serializable]
		public class FingerData : LeanFingerData
		{
			public List<Vector2> Points = new List<Vector2>(); // This stores the current shape this finger has drawn.

			public Vector2 EndPoint
			{
				get
				{
					return Points[Points.Count - 1];
				}
			}
		}

		public enum DirectionType
		{
			Forward,
			Backward,
			ForwardAndBackward
		}

		/// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		[Space]

		[Tooltip("The shape we want to detect.")]
		public LeanShape Shape;

		[Tooltip("The finger must move at least this many scaled pixels for it to record a new point.")]
		public float StepThreshold = 1.0f;

		[Tooltip("The drawn shape must be within this distance of the reference shape to be recognised. This is in local space relative to the reference shape.")]
		public float DistanceThreshold = 1.0f;

		[Tooltip("If you draw outside the DistanceThreshold, the error factor will increase based on how far you stray, until eventually the shape fails to detect. This allows you to set how high the error factor can become before the detection fails.")]
		public float ErrorThreshold = 1.0f;

		[Tooltip("If you want to allow partial shape matches, then specify the minimum amount of edges that must be matched in the shape.")]
		public int MinimumPoints = -1;

		[Tooltip("Which direction should the shape be checked using?")]
		public DirectionType Direction = DirectionType.ForwardAndBackward;

		/// <summary>If the finger goes up and it has traced the specified shape, this event will be invoked with the finger data.</summary>
		public LeanFingerEvent OnDetected { get { if (onDetected == null) onDetected = new LeanFingerEvent(); return onDetected; } } [SerializeField] private LeanFingerEvent onDetected;

		// This stores the currently active finger data.
		private List<FingerData> fingerDatas;

		// Pool the FingerData so we reduce GC alloc!
		private static Stack<FingerData> fingerDataPool = new Stack<FingerData>();

		private static List<Vector2Int> ranges = new List<Vector2Int>();

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually add a finger.</summary>
		public void AddFinger(LeanFinger finger)
		{
			var fingerData = LeanFingerData.FindOrCreate(ref fingerDatas, finger);

			fingerData.Points.Clear();

			fingerData.Points.Add(finger.ScreenPosition);
		}

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove a finger.</summary>
		public void RemoveFinger(LeanFinger finger)
		{
			LeanFingerData.Remove(fingerDatas, finger, fingerDataPool);
		}

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove all fingers.</summary>
		public void RemoveAllFingers()
		{
			LeanFingerData.RemoveAll(fingerDatas, fingerDataPool);
		}

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerDown   += HandleFingerDown;
			LeanTouch.OnFingerUpdate += HandleFingerUpdate;
			LeanTouch.OnFingerUp     += HandleFingerUp;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerDown   -= HandleFingerDown;
			LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
			LeanTouch.OnFingerUp     -= HandleFingerUp;
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.matrix = transform.localToWorldMatrix;

			if (Shape != null)
			{
				for (var i = 1; i < Shape.Points.Count; i++)
				{
					Gizmos.DrawLine(Shape.Points[i - 1], Shape.Points[i]);
				}

				foreach (var point in Shape.Points)
				{
					Gizmos.DrawWireSphere(point, DistanceThreshold);
				}
			}
		}
#endif

		private void HandleFingerDown(LeanFinger finger)
		{
			var fingers = Use.GetFingers();

			if (fingers.Contains(finger) == true)
			{
				AddFinger(finger);
			}
		}

		private void HandleFingerUpdate(LeanFinger finger)
		{
			var fingerData = LeanFingerData.Find(fingerDatas, finger);

			if (fingerData != null && Vector2.Distance(finger.ScreenPosition, fingerData.EndPoint) > StepThreshold)
			{
				fingerData.Points.Add(finger.ScreenPosition);
			}
		}

		private void HandleFingerUp(LeanFinger finger)
		{
			var fingerData = LeanFingerData.Find(fingerDatas, finger);
			var points     = fingerData.Points;

			LeanFingerData.Remove(fingerDatas, finger, fingerDataPool);

			if (Shape != null)
			{
				ranges.Clear();

				var min = Shape.Points.Count;
				var max = Shape.Points.Count;

				if (MinimumPoints > 0)
				{
					min = MinimumPoints;
				}

				for (var i = max; i >= min; i--)
				{
					if (Shape.ConnectEnds == true)
					{
						for (var j = 0; j < Shape.Points.Count; j++)
						{
							AddRange(j, j + i - 1);
						}
					}
					else
					{
						var steps = Shape.Points.Count - i;

						for (var j = 0; j <= steps; j++)
						{
							AddRange(j, j + i - 2);
						}
					}
				}

				foreach (var range in ranges)
				{
					if (CalculateMatch(points, Shape.Points, DistanceThreshold, ErrorThreshold, range.x, range.y) == true)
					{
						if (onDetected != null)
						{
							onDetected.Invoke(finger);
						}

						return;
					}
				}
			}
		}

		private void AddRange(int min, int max)
		{
			if (Direction == DirectionType.Forward || Direction == DirectionType.ForwardAndBackward)
			{
				ranges.Add(new Vector2Int(min, max));
			}

			min++;
			max++;

			if (Direction == DirectionType.Backward || Direction == DirectionType.ForwardAndBackward)
			{
				ranges.Add(new Vector2Int(max, min));
			}
		}

		struct Line
		{
			public Vector2 A;
			public Vector2 B;

			public float GetFirstDistance(Vector2 point)
			{
				return Vector2.Distance(point, A);
			}

			public float GetDistance(Vector2 point)
			{
				if (A == B) return Vector2.Distance(point, A);

				var v  = B - A;
				var w  = point - A;
				var c1 = Vector2.Dot(w,v); if (c1 <= 0.0f) return Vector2.Distance(point, A);
				var c2 = Vector2.Dot(v,v); if (c2 <= c1) return Vector2.Distance(point, B);
				var b  = c1 / c2;
				var Pb = A + b * v;

				return Vector2.Distance(point, Pb);
			}
		}

		private static Queue<Vector2> fittedShape = new Queue<Vector2>();

		private static Queue<Line> referenceLines = new Queue<Line>();

		private static bool CalculateMatch(List<Vector2> drawnShape, List<Vector2> referenceShape, float distanceThreshold, float errorThreshold, int min = -1, int max = -1)
		{
			if (drawnShape.Count > 1 && referenceShape.Count > 1 && distanceThreshold > 0.0f)
			{
				// drawnShape -> fittedShape
				FitShape(drawnShape, referenceShape);

				// referenceShape -> referenceLines
				ConvertPoints(referenceShape, min, max);

				/*
				for (var i = 1; i < referenceShape.Count; i++)
				{
					Debug.DrawLine(referenceShape[i - 1], referenceShape[i], Color.black, 5.0f);
				}
				var fittedShape2 = new List<Vector2>(fittedShape);
				for (var i = 1; i < fittedShape2.Count; i++)
				{
					var a = fittedShape2[i - 1];
					var b = fittedShape2[i];
					Debug.DrawLine(a, b, Color.magenta, 5.0f);
				}
				foreach (var a in fittedShape2)
				{
					Debug.DrawLine(a, a - Vector2.right * distanceThreshold, Color.magenta, 5.0f);
					Debug.DrawLine(a, a + Vector2.right * distanceThreshold, Color.magenta, 5.0f);
					Debug.DrawLine(a, a - Vector2.up * distanceThreshold, Color.magenta, 5.0f);
					Debug.DrawLine(a, a + Vector2.up * distanceThreshold, Color.magenta, 5.0f);
				}
				*/

				var line    = referenceLines.Dequeue();
				var penalty = 0.0f;
				var prevPos = fittedShape.Peek();

				while (fittedShape.Count > 0)
				{
					var point = fittedShape.Dequeue();
					var dist  = line.GetDistance(point);

					// Move to next line?
					if (referenceLines.Count > 0 && referenceLines.Peek().GetDistance(point) <= distanceThreshold)
					{
						line = referenceLines.Dequeue();
					}
					// Too far from current line?
					else if (dist > distanceThreshold)
					{
						penalty += ((dist - distanceThreshold) / distanceThreshold) * Vector2.Distance(prevPos, point);
					}

					// Used to calculate the distance between drawn points
					prevPos = point;
				}

				return referenceLines.Count == 0 && penalty < errorThreshold;
			}

			return false;
		}

		private static void FitShape(List<Vector2> drawnShape, List<Vector2> referenceShape)
		{
			fittedShape.Clear();

			var drawnRect     = GetRect(drawnShape);
			var referenceRect = GetRect(referenceShape);
			var scale         = referenceRect.size / drawnRect.size;

			foreach (var drawnPoint in drawnShape)
			{
				var point = drawnPoint;

				point -= drawnRect.center;
				point *= scale;
				point += referenceRect.center;

				fittedShape.Enqueue(point);
			}
		}

		private static void ConvertPoints(List<Vector2> referenceShape, int min, int max)
		{
			referenceLines.Clear();

			var count = Mathf.Abs(min - max) + 1;
			var step  = max > min ? 1 : -1;
			var last  = default(Vector2);

			for (var i = 0; i < count; i++)
			{
				var a = Read(referenceShape, min + step * i);
				var b = Read(referenceShape, min + step * i + step);

				if (referenceLines.Count == 0)
				{
					referenceLines.Enqueue(new Line() { A = a, B = a } );
				}

				referenceLines.Enqueue(new Line() { A = a, B = b } );

				last = b;
			}

			referenceLines.Enqueue(new Line() { A = last, B = last });
		}

		private static Vector2 Read(List<Vector2> list, int index)
		{
			return list[(index % list.Count + list.Count) % list.Count];
		}

		private static Rect GetRect(List<Vector2> shape)
		{
			var rect = new Rect(shape[0], Vector2.zero);

			foreach (var point in shape)
			{
				rect.xMin = Mathf.Min(rect.xMin, point.x);
				rect.yMin = Mathf.Min(rect.yMin, point.y);

				rect.xMax = Mathf.Max(rect.xMax, point.x);
				rect.yMax = Mathf.Max(rect.yMax, point.y);
			}

			return rect;
		}
	}
}