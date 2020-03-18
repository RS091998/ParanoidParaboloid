#region using   

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

#endregion 

namespace SpecularShader_Test
{

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        #region variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        
        //Shader Data

        Effect effect;
        Effect normalshader; 
       
        
        //Matrices

        Matrix world = Matrix.CreateTranslation(0, 0, 0);
        Matrix world1 = Matrix.CreateTranslation(0, 0, 0);
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1));
       // Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 600f, 0.1f, 100f);
        Matrix projection = Matrix.CreateOrthographicOffCenter(-10,10,10,-10, 0.1f, 100f);
        Matrix worldviewprojection;
        Matrix worldviewprojection1;

        RenderTarget2D rendertarget; 

        //Parameters to be changed by user input

        float angle = 0;
        float objangle = 0;
        float objangle1 = 0;
        float distance = 1;
        float Z = 0;
        float X = 0;
        float Y = 0;
        Boolean OtherShader = false; 

        //Models 

        Model model;

        //Display of Coordinate position

        SpriteFont font;

        #endregion


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        #region init 
        protected override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load Model and Paraboloid Shader
            model = Content.Load<Model>("ObjTest_Paper");
            effect = Content.Load<Effect>("Parashader");
            normalshader = Content.Load<Effect>("File");
            //Load Font for Coordinates
            font = Content.Load<SpriteFont>("This");

           

        }

        protected override void UnloadContent()
        {
        }
        #endregion

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            




            #region Matrixupdate
            view = Matrix.CreateLookAt(distance * new Vector3((float)Math.Sin(angle), 0, (float)Math.Cos(angle)), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            world1 = Matrix.CreateTranslation(X, Y, Z)* Matrix.CreateRotationY(objangle);
            #endregion 
            #region UserInterface 

            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Left))
            { 
                objangle += 0.01f;
            }
            if (state.IsKeyDown(Keys.Right))
            {
                objangle -= 0.01f;
            }
            if (state.IsKeyDown(Keys.Up))
            {
                distance -= 0.1f;
            }
            if (state.IsKeyDown(Keys.Down))
            {
                distance += 0.1f;
            }
            if (state.IsKeyDown(Keys.W))
            {
                Z += 0.1f;

            }
            if (state.IsKeyDown(Keys.A))
            {
                X += 0.01f;

            }
            if (state.IsKeyDown(Keys.D))
            {
                X -= 0.01f;

            }
            if (state.IsKeyDown(Keys.S))
            {
                Z -= 0.1f;

            }
            
            if (state.IsKeyDown(Keys.Q))
            {
                OtherShader = true;
            }
            
            if (state.IsKeyDown(Keys.E))
            { OtherShader = false; 
            
            
            }
            if (state.IsKeyDown(Keys.Back))
            {   objangle = 0;
                X = 0;
                Y = 0;
                Z = 0;
                distance = 0;
                angle = 0; 
            }
            #endregion

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            

            worldviewprojection = world1 * view * projection;

            worldviewprojection1 = world1 * view*projection;
           
            DrawToRendertarget(rendertarget);

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            spriteBatch.Draw(rendertarget, new Rectangle(0, 0, 800, 480), Color.Red);
            
            Color[] colors1D = new Color[800 * 480]; 
            
            spriteBatch.End();
            /*if (shouldCalculate)
            {
                renderTarget.GetData<Color>(colors1D);
                CalculateViewFactor(colors1D);
                shouldCalculate = !shouldCalculate;
            }

            

            //Select Shaders for comparison 

            if (OtherShader == false)
            {
                DrawModelWithEffect2(model, worldviewprojection1);
            }
            else
            {   DrawModelWithEffect(model, worldviewprojection);
            }*/




            //DrawNmodels(model, worldviewprojection, 5); 

            DisplayCoordinates(font, X, Y, Z);
            
            base.Draw(gameTime);
        }

        #region Drawmethods
        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = world * mesh.ParentBone.Transform;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
        }

        private void DrawModelWithEffect(Model model, Matrix worldviewprojection)
        {

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    
                    part.Effect = normalshader;
                    normalshader.Parameters["WorldViewProjection"].SetValue(worldviewprojection);
                  
                }
                mesh.Draw();
            }
        }

       
        private void DrawModelWithEffect2(Model model, Matrix worldviewprojection)
        {

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    
                    part.Effect = effect;
                    effect.Parameters["WorldViewProjection"].SetValue(worldviewprojection);
                   
                }
                mesh.Draw();
            }
        }
        private void DrawNmodels(Model model, Matrix worldviewprojection, int n)
        {

            for (int i = 0; i <= n; i++)
            {
                float zahl = 8.5f;
                world = Matrix.CreateTranslation(20 * (float)Math.Sin(3.141 * i / n), 20 * (float)Math.Cos(3.141 * i / n), 0);


                worldviewprojection = world * view * projection;


                DrawModelWithEffect2(model, worldviewprojection);
            }

        }

        private void DrawToRendertarget(RenderTarget2D rendertarget)
        {   GraphicsDevice.SetRenderTarget(rendertarget);

            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            GraphicsDevice.Clear(Color.Black); 
            DrawModelWithEffect2(model, worldviewprojection1);
            

            GraphicsDevice.SetRenderTarget(null); 
        }
        #endregion

        private void DisplayCoordinates(SpriteFont font, float X, float Y, float Z)
        {
            spriteBatch.Begin();

            spriteBatch.DrawString(font, "X = " + X, new Vector2(50, 20), Color.White);
            spriteBatch.DrawString(font, "Z = " + Z, new Vector2(50, 70), Color.White);
            spriteBatch.DrawString(font, "Y = " + Y, new Vector2(50, 120), Color.White);
            spriteBatch.DrawString(font, "dist = " + distance, new Vector2(50, 170), Color.White);
            spriteBatch.End();
        }

    }

    }

