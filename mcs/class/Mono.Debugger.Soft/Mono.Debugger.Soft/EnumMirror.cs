using System;
using System.Collections.Generic;

namespace Mono.Debugger.Soft
{
	/*
	 * Represents an enum value in the debuggee
	 */
	public class EnumMirror : StructMirror {
	
		internal EnumMirror (VirtualMachine vm, TypeMirror type, Value[] fields) : base (vm, type, fields) {
		}

		internal EnumMirror (VirtualMachine vm, TypeMirror type, PrimitiveValue value) : base (vm, type, new Value[] { value }) {
			if (type == null)
				throw new ArgumentNullException ("type");
			if (value == null)
				throw new ArgumentNullException ("value");
			if (!type.IsEnum)
				throw new ArgumentException ("type must be an enum type", "type");
			TypeMirror t = GetUnderlyingType (type);
			if (value.Value == null || !value.Value.GetType ().IsPrimitive || t != vm.RootDomain.GetCorrespondingType (value.Value.GetType ()))
				throw new ArgumentException ("Value '" + value.Value + "' does not match the type of the enum.");
		}

		public object Value {
			get {
				return ((PrimitiveValue)Fields [0]).Value;
			}
			set {
				SetField (0, vm.CreateValue (value));
			}
		}

		public string StringValue {
			get {
				foreach (FieldInfoMirror f in Type.GetFields ()) {
					if (f.IsStatic) {
						object v = (Type.GetValue (f) as EnumMirror).Value;
						if (f.IsStatic && v.Equals (Value))
							return f.Name;
					}
				}
				return Value.ToString ();
			}
		}

		public TypeMirror GetUnderlyingType (TypeMirror t) {
			foreach (FieldInfoMirror f in t.GetFields ()) {
				if (!f.IsStatic)
					return f.FieldType;
			}
			throw new NotImplementedException ();
		}
	}
}