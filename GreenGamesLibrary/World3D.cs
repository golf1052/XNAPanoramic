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
    public class World3D
    {
        public float aspectRatio;
        public Vector3 camPos = Vector3.Zero;

        public World3D(GraphicsDevice graphics)
        {
            aspectRatio = graphics.Viewport.AspectRatio;
        }
    }
}
