using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Demo_MG_Levels
{
    public class Portal
    {
        #region FIELDS

        private ContentManager _contentManager;
        private string _spriteName;
        private Texture2D _sprite;
        private Vector2 _position;
        private Vector2 _center;
        private Rectangle _boundingRectangle;
        private float rotation;

        private bool _active;

        #endregion

        #region PROPERTIES

        public ContentManager ContentManager
        {
            get { return _contentManager; }
            set { _contentManager = value; }
        }

        public string SpriteName
        {
            get { return _spriteName; }
            set { _spriteName = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                _center = new Vector2(_position.X + (_sprite.Width / 2), _position.Y + (_sprite.Height / 2));
                _boundingRectangle = new Rectangle((int)_position.X, (int)_position.Y, _sprite.Width, _sprite.Height);
            }
        }

        public Vector2 Center
        {
            get { return _center; }
            set { _center = value; }
        }

        public Rectangle BoundingRectangle
        {
            get { return _boundingRectangle; }
            set { _boundingRectangle = value; }
        }

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// instantiate a new Portal
        /// </summary>
        /// <param name="contentManager">game content manager object</param>
        /// <param name="spriteName">file name of sprite</param>
        /// <param name="position">vector position of Portal</param>
        public Portal(
            ContentManager contentManager,
            string spriteName,
            Vector2 position
            )
        {
            _contentManager = contentManager;
            _spriteName = spriteName;
            _position = position;
            _active = true;

            // load the Portal image into the Texture2D for the Portal sprite
            _sprite = _contentManager.Load<Texture2D>(_spriteName);

            // set the initial center and bounding rectangle for the Portal
            _center = new Vector2(position.X + (_sprite.Width / 2), position.Y + (_sprite.Height / 2));
            _boundingRectangle = new Rectangle((int)position.X, (int)position.Y, _sprite.Width, _sprite.Height);
        }

        #endregion

        #region METHODS
        /// <summary>
        /// add Portal sprite to the SpriteBatch object
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            rotation += .1F;

            // only draw the Portal if it is active
            if (_active)
            {
                spriteBatch.Draw(_sprite, _center, null, Color.White, rotation, new Vector2(32, 32), 1, SpriteEffects.None, 0);
            }
        }

        #endregion
    }
}
