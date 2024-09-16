using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Copyright (c) 2015 burningmime
// 
// This software is provided 'as-is', without any express or implied
// warranty. In no event will the authors be held liable for any damages
// arising from the use of this software.
// 
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
// 
// 1. The origin of this software must not be misrepresented; you must not
//    claim that you wrote the original software. If you use this software
//    in a product, an acknowledgement in the product documentation would be
//    appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
//    misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.

using System.Runtime.CompilerServices;

using VECTOR = UnityEngine.Vector3;
using FLOAT = System.Single;


    /// <summary>
    /// The point of this class is to abstract some of the functions of Vector2 so they can be used with System.Windows.Vector,
    /// System.Numerics.Vector2, UnityEngine.Vector2, or another vector type.
    /// </summary>
    public static class VectorHelper
    {
        /// <summary>
        /// Below this, don't trust the results of floating point calculations.
        /// </summary>
        public const FLOAT EPSILON = 1.2e-12f;


        public static FLOAT Distance(VECTOR a, VECTOR b) { return VECTOR.Distance(a, b); }
        public static FLOAT DistanceSquared(VECTOR a, VECTOR b) { float dx = a.x - b.x; float dy = a.y - b.y; float dz = a.z - b.z; return dx*dx + dy*dy + dz*dz; }
        public static FLOAT Dot(VECTOR a, VECTOR b) { return VECTOR.Dot(a, b); }
        public static VECTOR Normalize(VECTOR v) { v.Normalize(); return v; }
        public static FLOAT Length(VECTOR v) { return v.magnitude; }
        public static FLOAT LengthSquared(VECTOR v) { return v.sqrMagnitude; }
        public static VECTOR Lerp(VECTOR a, VECTOR b, FLOAT amount) { return VECTOR.Lerp(a, b, amount); }
        public static FLOAT GetX(VECTOR v) { return v.x; }
        public static FLOAT GetY(VECTOR v) { return v.y; }
        public static FLOAT GetZ(VECTOR v) { return v.z; }



        /// <summary>
        /// Checks if two vectors are equal within a small bounded error.
        /// </summary>
        /// <param name="v1">First vector to compare.</param>
        /// <param name="v2">Second vector to compare.</param>
        /// <returns>True iff the vectors are almost equal.</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public static bool EqualsOrClose(VECTOR v1, VECTOR v2)
        {
            return DistanceSquared(v1, v2) < EPSILON;
        }
    }
