
public class PID {
	
	// Properties
	public float P { get; set; }
	public float I { get; set; }
	public float D { get; set; }
	public float MaxIntegral { get; set; } = float.MaxValue;

	// Constructor
	public PID (float p, float i, float d) {
		P = p;
		I = i;
		D = d;
	}
	
	/// <summary>Evaluates the controller</summary>
	/// <param name="error">The difference between the current state and the desired state of the system that is being controlled by this PID controller</param>
	/// <param name="deltaTime">Time from last update in seconds</param>
	/// <returns>The output value of the controller</returns>
	public float Update(float error, float deltaTime) {
		Integral = Clamp(Integral + I * deltaTime * error, MaxIntegral);
		float result = P * error + Integral - D * (error - LastValue) / deltaTime;
		LastValue = error;
		return result;
	}

	/// <summary>Clears the internal state of the controller</summary>
	public void Reset() {
		Integral = 0f;
		LastValue = 0f;
	}

	// Fields
	float Integral = 0f;
	float LastValue = 0f;
	
	// Private functions
	float Clamp(float value, float clamp) {
		return value > clamp ? clamp : value < -clamp ? -clamp : value;
	}
}

public class PID3 {
	public class Vector3 {
		public float X;
		public float Y;
		public float Z;

		public Vector3(float x, float y, float z) { X = x; Y = y; Z = z; }
		/// <summary>Shorthand for new(0f, 0f, 0f)</summary>
		public static Vector3 Zero => new(0f, 0f, 0f);

		// Functions
		public void Clamp(float maxLength) {
			float lengthSqr = X * X + Y * Y + Z * Z;
			if (lengthSqr < maxLength * maxLength) return;
			float scalar = maxLength / System.MathF.Sqrt(lengthSqr);
			X *= scalar;
			Y *= scalar;
			Z *= scalar;
		}

		// Operators
		// Add
		public static Vector3 operator +(Vector3 a, Vector3 b) {
			return new ( a.X + b.X, a.Y + b.Y, a.Z + b.Z );
		}
		// Subtract
		public static Vector3 operator -(Vector3 a, Vector3 b) {
			return new ( a.X - b.X, a.Y - b.Y, a.Z - b.Z );
		}
		// Multiply
		public static Vector3 operator *(Vector3 a, Vector3 b) {
			return new ( a.X * b.X, a.Y * b.Y, a.Z * b.Z );
		}
		public static Vector3 operator *(Vector3 a, float b) {
			return new ( a.X * b, a.Y * b, a.Z * b );
		}
		public static Vector3 operator *(float a, Vector3 b) {
			return new ( a * b.X, a * b.Y, a * b.Z );
		}
		// Divide
		public static Vector3 operator /(Vector3 a, Vector3 b) {
			return new ( a.X / b.X, a.Y / b.Y, a.Z / b.Z );
		}
		public static Vector3 operator /(Vector3 a, float b) {
			return new ( a.X / b, a.Y / b, a.Z / b );
		}
		public static Vector3 operator /(float a, Vector3 b) {
			return new ( a / b.X, a / b.Y, a / b.Z );
		}
	}

	// Properties
	public float P { get; set; }
	public float I { get; set; }
	public float D { get; set; }
	public float MaxIntegral { get; set; } = float.MaxValue;

	// Constructor
	public PID3 (float p, float i, float d) {
		P = p;
		I = i;
		D = d;
	}

	/// <summary>Evaluates the controller</summary>
	/// <param name="error">The difference between the current state and the desired state of the system that is being controlled by this PID controller</param>
	/// <param name="deltaTime">Time from last update in seconds</param>
	/// <returns>The output value of the controller</returns>
	public Vector3 Update(Vector3 error, float deltaTime) {
		//V errorV = new(error.x, error.y, error.z);
		//integralX += I * deltaTime * error.x;
		Integral += I * deltaTime * error;
		Integral.Clamp(MaxIntegral);
		Vector3 last = LastValue;
		LastValue = error;
		return P * error + Integral - D * (error - last) / deltaTime;
	}

	/// <summary>Clears the internal state of the controller</summary>
	public void Reset() {
		Integral = Vector3.Zero;
		LastValue = Vector3.Zero;
	}

	// Fields
	Vector3 Integral = Vector3.Zero;
	Vector3 LastValue = Vector3.Zero;
}