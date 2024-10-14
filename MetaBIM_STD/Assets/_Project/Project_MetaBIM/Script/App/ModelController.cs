using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MetaBIM
{
    public class ModelController
    {


        public static void SetMeshFromBound(Bounds _bound, Mesh _mesh)
        {
            Vector3 boundPoint1 = _bound.min;
            Vector3 boundPoint2 = _bound.max;
            Vector3 boundPoint3 = new Vector3(boundPoint1.x, boundPoint1.y, boundPoint2.z);
            Vector3 boundPoint4 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint1.z);
            Vector3 boundPoint5 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint1.z);
            Vector3 boundPoint6 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint2.z);
            Vector3 boundPoint7 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint2.z);
            Vector3 boundPoint8 = new Vector3(boundPoint2.x, boundPoint2.y, boundPoint1.z);

            _mesh.vertices = new[] { boundPoint1, boundPoint2, boundPoint3, boundPoint4, boundPoint5, boundPoint6, boundPoint7, boundPoint8 };
            _mesh.triangles = new[]
                 {
                     0,7,4,
                     0,3,7,
                     5,1,3,
                     3,1,7,
                     7,1,4,
                     4,1,6,
                     5,3,2,
                     2,3,0,
                     0,4,2,
                     2,4,6,
                     1,5,2,
                     6,1,2
                 };

            
        }
    }
}
