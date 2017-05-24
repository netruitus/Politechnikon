using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Politechnikon.engine;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using Politechnikon.game_elements;

namespace Politechnikon.game_logic
{
    public enum GameState
    {
        InMenu,
        InGame,
        InEnd,
        InSave,
        InLoad,
        InHighScore,
        InNewGame,
        AreYouSure
    }

    public enum BufforClear
    {
        Everything,
        InterfaceObjects,
        GameObjects,
        TextObjects
    }

    public class Mechanic
    {
        private List<InitializedObjectTexture> ObjectsToRender;
        private List<InitializedObjectTexture> TextToRender;
        private List<InitializedObjectTexture> TextObjects;
        private List<InitializedObjectTexture> InterfaceObjects;
        private List<Text> Texts;
        private GameState gameState;
        private GameState lastGameState;
        private OpenTK.Input.MouseDevice Mouse;
        private Engine engine;
        private bool Loading;
        private bool QuitConfirm;
        private bool GameHasStarted;
        private bool GameRunning;
        public Mechanic(List<InitializedObjectTexture> objectList, List<InitializedObjectTexture> textList, Engine eng)
        {    
            this.QuitConfirm = false;
            this.gameState = GameState.InMenu;
            this.Loading = true; 
            this.ObjectsToRender = objectList;
            this.TextToRender = textList;
            this.engine = eng;
            this.Mouse = engine.Mouse;          
        }

        public void InitObjects()
        {
            //inicjalizacja obiektów i zmiennych
            this.InterfaceObjects = new List<InitializedObjectTexture>();
            this.TextObjects = new List<InitializedObjectTexture>();
            this.Texts = new List<Text>(); 
        }

        public void GetInput()
        {
        //zbieranie inputów myszy -- głównie zmiany stanów
            //zbieranie inputów, kiedy nic się nie ładuje
            if (Loading == false)
            {
                //kliknięcie lewym przyciskiem myszy
                if (Input.MousePress(OpenTK.Input.MouseButton.Left))
                {
                    //stan gry w menu
                    if (gameState == GameState.InMenu)
                    {
                        //kliknięcie zakończ
                        if (Mouse.X > 562 && Mouse.X < 737 && Mouse.Y > 329 && Mouse.Y < 368)
                        {
                            QuitConfirm = true;
                            lastGameState = gameState;
                            gameState = GameState.AreYouSure;
                            Loading = true;   
                        }
                        //kliknięcie w najwyższe wyniki
                        else if (Mouse.X > 562 && Mouse.X < 737 && Mouse.Y > 258 && Mouse.Y < 298)
                        {
                            lastGameState = gameState;
                            gameState = GameState.InHighScore;
                            Loading = true;
                        }
                        //kliknięcie w wczytaj grę
                        else if (Mouse.X > 562 && Mouse.X < 737 && Mouse.Y > 186 && Mouse.Y < 225)
                        {
                            lastGameState = gameState;
                            gameState = GameState.InLoad;
                            Loading = true;
                        }
                        //kliknięcie w nową grę
                        else if (Mouse.X > 562 && Mouse.X < 737 && Mouse.Y > 115 && Mouse.Y < 155)
                        {
                            lastGameState = gameState;
                            gameState = GameState.InNewGame;
                            Loading = true;
                        }
                    }
                    //stan gry w oknie nowej gry
                    else if (gameState == GameState.InNewGame)
                    {
                        //kliknięcie anuluj
                        if (Mouse.X > 656 && Mouse.X < 744 && Mouse.Y > 251 && Mouse.Y < 282)
                        {
                            gameState = GameState.InMenu;
                            lastGameState = GameState.InNewGame;
                            Loading = true;
                        }
                        //kliknięcie zatwierdź
                        else if (Mouse.X > 553 && Mouse.X < 641 && Mouse.Y > 251 && Mouse.Y < 282)
                        {
                            //zatwierdzenie przez gracza gry -- do zrobienia
                        }
                       //kliknięcie zakończ
                        else if (Mouse.X > 723 && Mouse.X < 793 && Mouse.Y > 4 && Mouse.Y < 28)
                        {
                            QuitConfirm = true;
                            lastGameState = gameState;
                            gameState = GameState.AreYouSure;
                            Loading = true; 
                        }
                        //kliknięcie wczytaj
                        else if (Mouse.X > 651 && Mouse.X < 720 && Mouse.Y > 4 && Mouse.Y < 28)
                        {
                            lastGameState = gameState;
                            gameState = GameState.InLoad;
                            Loading = true; 
                        }
                    }
                    //stan gry - czy jesteś pewien - wybór: tak/nie
                    else if (gameState == GameState.AreYouSure)
                    {
                        //wybór tak
                        if (Mouse.X > 552 && Mouse.X < 642 && Mouse.Y > 251 && Mouse.Y < 282)
                        {
                            if (QuitConfirm == true) engine.Exit();  
                        }
                        //wybór nie
                        else if (Mouse.X > 655 && Mouse.X < 745 && Mouse.Y > 251 && Mouse.Y < 282)
                        {
                            QuitConfirm = false;
                            gameState = GameState.InMenu;
                            lastGameState = GameState.AreYouSure;
                            Loading = true;
                        }
                    }
                    //stan gry - wczytywanie zapisu
                    else if (gameState == GameState.InLoad)
                    {
                        //kliknięcie zakończ
                        if (Mouse.X > 723 && Mouse.X < 793 && Mouse.Y > 4 && Mouse.Y < 28)
                        {
                            QuitConfirm = true;
                            lastGameState = gameState;
                            gameState = GameState.AreYouSure;
                            Loading = true; 
                        }
                        //kliknięcie wczytaj
                        else if (Mouse.X > 520 && Mouse.X < 595 && Mouse.Y > 401 && Mouse.Y < 432)
                        {

                        }
                        //kliknięcie usuń
                        else if (Mouse.X > 603 && Mouse.X < 679 && Mouse.Y > 401 && Mouse.Y < 432)
                        {

                        }
                        //kliknięcie anuluj
                        else if (Mouse.X > 685 && Mouse.X < 760 && Mouse.Y > 401 && Mouse.Y < 432)
                        {
                            gameState = GameState.InMenu;
                            lastGameState = GameState.InLoad;
                            Loading = true;
                        }
                        //kliknięcie górnej strzałki
                        else if (Mouse.X > 758 && Mouse.X < 773 && Mouse.Y > 264 && Mouse.Y < 313)
                        {

                        }
                        //kliknięcie dolnej strzałki
                        else if (Mouse.X > 758 && Mouse.X < 773 && Mouse.Y > 313 && Mouse.Y < 364)
                        {

                        }
                    }
                    //stan gry - zapis gry
                    else if (gameState == GameState.InSave)
                    {

                    }
                    //stan gry - highscore
                    else if (gameState == GameState.InHighScore)
                    {
                        //kliknięcie zakończ
                        if (Mouse.X > 723 && Mouse.X < 793 && Mouse.Y > 4 && Mouse.Y < 28)
                        {
                            QuitConfirm = true;
                            lastGameState = gameState;
                            gameState = GameState.AreYouSure;
                            Loading = true;
                        }
                        //kliknięcie nowa gra
                        if (Mouse.X > 505 && Mouse.X < 577 && Mouse.Y > 4 && Mouse.Y < 28)
                        {
                            lastGameState = gameState;
                            gameState = GameState.InNewGame;
                            Loading = true;
                        }
                        //kliknięcie wczytaj
                        if (Mouse.X > 651 && Mouse.X < 720 && Mouse.Y > 4 && Mouse.Y < 28)
                        {
                            lastGameState = gameState;
                            gameState = GameState.InLoad;
                            Loading = true;
                        }
                        //kliknięcie cofnij
                        if (Mouse.X > 604 && Mouse.X < 692 && Mouse.Y > 450 && Mouse.Y < 479)
                        {
                            lastGameState = gameState;
                            gameState = GameState.InMenu;
                            Loading = true;
                        }
                        //kliknięcie górnej strzałki
                        if (Mouse.X > 758 && Mouse.X < 772 && Mouse.Y > 351 && Mouse.Y < 399)
                        {

                        }
                        //kliknięcie dolnej strzałki
                        if (Mouse.X > 758 && Mouse.X < 772 && Mouse.Y > 400 && Mouse.Y < 450)
                        {

                        }
                    }
                    //stan gry - w grze
                    else if (gameState == GameState.InGame)
                    {

                    }
                }
            }
        }
        //ładowanie zawartości bufora obiektów interfejsu
        private void LoadToRenderInterfaceObjects()
        {
            for (int i = 0; i < InterfaceObjects.Count; i++)
            {
                ObjectsToRender.Add(InterfaceObjects[i]);
            }
        }

        private void LoadToRenderTextObjects()
        {
            for (int i = 0; i < TextObjects.Count; i++)
            {
                TextToRender.Add(TextObjects[i]);
            }
        }


        //czyszczenie renderera lub jego elementów
        private void ClearRenderer(BufforClear Option)
        {
            if (Option == BufforClear.Everything)
            {
                for (int i = 0; i < ObjectsToRender.Count; i++)
                {
                    ObjectsToRender.Remove(ObjectsToRender[i]);
                }   
            }
            else if (Option == BufforClear.InterfaceObjects)
            {
                for (int i = 0; i < InterfaceObjects.Count; i++)
                {
                    ObjectsToRender.Remove(InterfaceObjects[i]);
                }  
            }

        }
        //kasowanie zawartości bufora obiektów interfejsu
        private void ClearInterfaceObjectBuffer()
        {
            for (int i = 0; i < InterfaceObjects.Count; i++)
            {
                InterfaceObjects.Remove(InterfaceObjects[i]);
            }
        }

        private void ClearTextRenderer()
        {
            for (int i = 0; i < TextToRender.Count; i++)
            {
                TextToRender.Remove(TextToRender[i]);
            }
        }

        private void ClearTextObjectBuffer()
        {
            for (int i = 0; i < TextObjects.Count; i++)
            {
                TextObjects.Remove(TextObjects[i]);
            }
        }


        public void update()
        {
            //reakcje na zmiany stanów i inputy
            if (gameState == GameState.InMenu)
            {
                if (Loading == true)
                {
                    Loading = false;
                    LoadGuiTexture(500, 0, 06);
                    RenderReload();
                }        
            }
            else if (gameState == GameState.InNewGame)
            {
                if (Loading == true)
                {
                    Loading = false;
                    LoadGuiTexture(500, 0, 08);
                    LoadText(525, 135, 25, "Nazwij swojego avatara");           
                    RenderReload();
                }  
            }
            else if (gameState == GameState.AreYouSure)
            {
                if (Loading == true)
                {
                    Loading = false;
                    LoadGuiTexture(500, 0, 03);
                    LoadText(515, 190, 18, "Czy na pewno chcesz zakończyć grę?");
                    RenderReload();
                }
            }
            else if (gameState == GameState.InLoad)
            {
                if (Loading == true)
                {
                    Loading = false;
                    LoadGuiTexture(500, 0, 10);
                    RenderReload();
                }
            }
            else if (gameState == GameState.InHighScore)
            {
                if (Loading == true)
                {
                    Loading = false;
                    LoadGuiTexture(500, 0, 07);
                    RenderReload();
                }
            }
        }

        private void LoadText(int x, int y, int fontSize, String txt)
        {
            Text tekst = new Text(x, y, fontSize, txt);
            InitializedObjectTexture tekstTexture = new InitializedObjectTexture(tekst.X, tekst.Y, tekst.SizeX, tekst.SizeY, tekst.GeneratedBMP, Color.White);
            TextObjects.Add(tekstTexture);
        }

        private void LoadGuiTexture(int x, int y, int id)
        {
            Background background = new Background(x, y, id, BackgroundType.GUI);
            InitializedObjectTexture Menu = new InitializedObjectTexture(background.X, background.Y, background.SizeX, background.SizeY, background.Path, Color.White);
            InterfaceObjects.Add(Menu);
        }




        private void RenderReload()
        {
            ClearRenderer(BufforClear.Everything);
            ClearTextRenderer();
            LoadToRenderInterfaceObjects();
            LoadToRenderTextObjects();
            ClearInterfaceObjectBuffer();
            ClearTextObjectBuffer();
        }


    }
}
