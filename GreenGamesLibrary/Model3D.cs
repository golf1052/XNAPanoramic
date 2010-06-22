using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace GreenGamesLibrary
{
    public class Model3D
    {
        public Model object3D;
        public Matrix[] transforms;
        public Vector3 pos = Vector3.Zero;
        public Vector3 vel = Vector3.Zero;
        public BoundingBox box;
        public BoundingSphere sphere;

        public Model3D(Model loadedModel)
        {
            object3D = loadedModel;
            transforms = new Matrix[object3D.Bones.Count];
            object3D.CopyAbsoluteBoneTransformsTo(transforms);
        }

        public void DrawModel()
        {
            foreach (ModelMesh mesh in object3D.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                }
            }
        }
    }
}
