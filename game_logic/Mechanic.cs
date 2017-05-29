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
using OpenTK.Input;

/* Do zrobienia:
 * - zmiany na levele + boss levele (moc potworów uzalezniona odlevea aktualnego)
 * - zuzywanie itemków dają doświadczenie w przypadku zużycia - prawy przycisk
 * - obsługa zmiennych
 * - może save i loady?
 * - warunki na eksplorację
 * - lootowanie z mobków itemów
 * - opracowanie statystyk i poprawa baz danych
 * - opaczyć grafikę
 * - warunki na przechodzenie do kolejnych leveli - wzięcie klucza
 * - obsługa ekwipunku - zakładanie, plecaka etc.
 * - levelowanie postaci
 * - wygrana i przegrana gry
 */



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
        AreYouSure,
        AreYouSureInGame,
        NewGameInGame,
        LoadInGame,
        EndGameLose,
        EndGameWin
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
        private List<InitializedObjectTexture> GameObjects;
        private List<InitializedObjectTexture> ObjectsToRemove;
        private List<Item> GeneratedItems;
        private List<Monster> GeneratedMonsters;
        private List<Text> Texts;
        private InitializedObjectTexture LastTmpText;
        private InitializedObjectTexture TmpText;
        private List<InitializedObjectTexture> StatusDisplayText;
        private GameState gameState;
        private GameState lastGameState;
        private Avatar avatar;
        private Field[,] fields;
        private OpenTK.Input.MouseDevice Mouse;
        private Engine engine;
        private String PlayersName;
        private bool Loading;
        private bool QuitConfirm;
        private bool ReloadInterface;
        private bool GameHasStarted;
        private bool GameRunning;
        private Item ReferenceItem;
        private Monster ReferenceMonster;
        private int Boss_Key_X;
        private int Boss_Key_Y;
        private int LevelUpPeak;
        
        public Mechanic(List<InitializedObjectTexture> objectList, List<InitializedObjectTexture> textList, Engine eng)
        {    
            this.QuitConfirm = false;
            this.gameState = GameState.InMenu;
            this.Loading = true; 
            this.ObjectsToRender = objectList;
            this.TextToRender = textList;
            this.engine = eng;
            engine.KeyPress += engine_KeyPress;
            this.Mouse = engine.Mouse;          
        }

        public void InitObjects()
        {
            //inicjalizacja obiektów i zmiennych
            this.ReloadInterface = false;
            this.InterfaceObjects = new List<InitializedObjectTexture>();
            this.TextObjects = new List<InitializedObjectTexture>();
            this.ObjectsToRemove = new List<InitializedObjectTexture>();
            this.GameObjects = new List<InitializedObjectTexture>();
            this.StatusDisplayText = new List<InitializedObjectTexture>();
            this.GeneratedItems = new List<Item>();
            this.GeneratedMonsters = new List<Monster>();
            this.Texts = new List<Text>();
            this.LevelUpPeak = 100;
            PlayersName = "";
            TmpText = LoadTextInput(1, 1, 3, " ");
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
                            if (PlayersName.Equals("") || PlayersName.Equals(" "))
                            {
                                var tmp = LoadTextInput(510, 285, 21, "Wpisz imię dla swojego avatara!", Color.Red);
                                ObjectsToRemove.Add(tmp);
                            }
                            else
                            {
                                gameState = GameState.InGame;
                                lastGameState = GameState.InNewGame;
                                Loading = true;
                            }
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
                        else if (Mouse.X > 505 && Mouse.X < 577 && Mouse.Y > 4 && Mouse.Y < 28)
                        {
                            lastGameState = gameState;
                            gameState = GameState.InNewGame;
                            Loading = true;
                        }
                        //kliknięcie wczytaj
                        else if (Mouse.X > 651 && Mouse.X < 720 && Mouse.Y > 4 && Mouse.Y < 28)
                        {
                            lastGameState = gameState;
                            gameState = GameState.InLoad;
                            Loading = true;
                        }
                        //kliknięcie cofnij
                        else if (Mouse.X > 604 && Mouse.X < 692 && Mouse.Y > 450 && Mouse.Y < 479)
                        {
                            lastGameState = gameState;
                            gameState = GameState.InMenu;
                            Loading = true;
                        }
                        //kliknięcie górnej strzałki
                        else if (Mouse.X > 758 && Mouse.X < 772 && Mouse.Y > 351 && Mouse.Y < 399)
                        {

                        }
                        //kliknięcie dolnej strzałki
                        else if (Mouse.X > 758 && Mouse.X < 772 && Mouse.Y > 400 && Mouse.Y < 450)
                        {

                        }
                    }
                    //stan gry - czy jesteś pewien - wybór: tak/nie w grze
                    else if (gameState == GameState.AreYouSureInGame)
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
                            gameState = GameState.InGame;
                            lastGameState = GameState.AreYouSure;
                            ReloadInterface = true;
                        }
                    }
                    //stan gry - czy jesteś pewien, żeby nową grę rozpocząć - wybór: tak/nie w grze
                    else if (gameState == GameState.NewGameInGame)
                    {
                        //wybór tak
                        if (Mouse.X > 552 && Mouse.X < 642 && Mouse.Y > 251 && Mouse.Y < 282)
                        {
                            lastGameState = gameState;
                            gameState = GameState.InNewGame;
                            Loading = true;
                        }
                        //wybór nie
                        else if (Mouse.X > 655 && Mouse.X < 745 && Mouse.Y > 251 && Mouse.Y < 282)
                        {
                            QuitConfirm = false;
                            gameState = GameState.InGame;
                            lastGameState = GameState.NewGameInGame;
                            ReloadInterface = true;
                        }
                    }
                    //stan gry - wczytywanie zapisu w grze
                    else if (gameState == GameState.LoadInGame)
                    {
                        //kliknięcie zakończ
                        if (Mouse.X > 723 && Mouse.X < 793 && Mouse.Y > 4 && Mouse.Y < 28)
                        {
                            QuitConfirm = true;
                            lastGameState = gameState;
                            gameState = GameState.AreYouSureInGame;
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
                            gameState = GameState.InGame;
                            lastGameState = GameState.LoadInGame;
                            ReloadInterface = true;
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
                    else if (gameState == GameState.InSave)
                    {
                        //kliknięcie zakończ
                        if (Mouse.X > 723 && Mouse.X < 793 && Mouse.Y > 4 && Mouse.Y < 28)
                        {
                            QuitConfirm = true;
                            lastGameState = gameState;
                            gameState = GameState.AreYouSureInGame;
                            Loading = true;
                        }
                        //kliknięcie wczytaj
                        else if (Mouse.X > 651 && Mouse.X < 720 && Mouse.Y > 4 && Mouse.Y < 28)
                        {
                            lastGameState = gameState;
                            gameState = GameState.LoadInGame;
                            Loading = true;
                        }
                        //kliknięcie zapisz
                        else if (Mouse.X > 520 && Mouse.X < 595 && Mouse.Y > 445 && Mouse.Y < 476)
                        {

                        }
                        //kliknięcie usuń
                        else if (Mouse.X > 603 && Mouse.X < 679 && Mouse.Y > 445 && Mouse.Y < 476)
                        {

                        }
                        //kliknięcie anuluj
                        else if (Mouse.X > 685 && Mouse.X < 760 && Mouse.Y > 445 && Mouse.Y < 476)
                        {
                            gameState = GameState.InGame;
                            lastGameState = GameState.InSave;
                            ReloadInterface = true;
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
                    //stan gry - w grze
                    else if (gameState == GameState.InGame)
                    {
                        //kliknięcie zakończ
                        if (Mouse.X > 723 && Mouse.X < 793 && Mouse.Y > 4 && Mouse.Y < 28)
                        {
                            QuitConfirm = true;
                            lastGameState = gameState;
                            gameState = GameState.AreYouSureInGame;
                            Loading = true;
                        }
                        //kliknięcie nowa gra
                        else if (Mouse.X > 505 && Mouse.X < 577 && Mouse.Y > 4 && Mouse.Y < 28)
                        {
                            lastGameState = gameState;
                            gameState = GameState.NewGameInGame;
                            Loading = true;
                        }
                        //kliknięcie wczytaj
                        else if (Mouse.X > 651 && Mouse.X < 720 && Mouse.Y > 4 && Mouse.Y < 28)
                        {
                            lastGameState = gameState;
                            gameState = GameState.LoadInGame;
                            Loading = true;
                        }
                        //kliknięcie zapisz grę
                        else if (Mouse.X > 578 && Mouse.X < 648 && Mouse.Y > 4 && Mouse.Y < 28)
                        {
                            lastGameState = gameState;
                            gameState = GameState.InSave;
                            Loading = true;
                        }
                        else if (Mouse.X > 0 && Mouse.X < 500 && Mouse.Y > 0 && Mouse.Y < 500)
                        {
                            ///eksploracja pól
                            Field field = fields[(int)Mouse.X / 50, (int)Mouse.Y / 50];
                            if (field.Explored == false) 
                            {

                                //warunek na explorację



                                field.Explored = true;
                                var tmpToAdd = LoadFieldTexture(field.X / 50, field.Y / 50, field.Id, field.Explored);
                                ObjectsToRender.Add(tmpToAdd);
                                InitializedObjectTexture tmpToDisplay = null;
                                Random rng = new Random();
                                if (field.Id == 4)
                                {
                                    if (field.X / 50 == Boss_Key_X && field.Y / 50 == Boss_Key_Y)
                                    {
                                        tmpToDisplay = LoadItemTexture(field.X / 50, field.Y / 50, 0, ItemType.Others);
                                        fields[field.X / 50, field.Y / 50].Description = ReferenceItem.Description;
                                    }
                                    else
                                    {
                                        ItemType type; 
                                        if (Arena.ChanceRandom(25)) type = ItemType.Armor;
                                        else if (Arena.ChanceRandom(33.33)) type = ItemType.Weapon;
                                        else type = ItemType.Others;

                                        if (type == ItemType.Armor)
                                        {
                                            tmpToDisplay = LoadItemTexture(field.X / 50, field.Y / 50, (rng.Next(1, 6) * 100) + 1, ItemType.Armor);
                                            fields[field.X / 50, field.Y / 50].Description = ReferenceItem.Description;
                                            fields[field.X / 50, field.Y / 50].TextBuffor = "Obrona: " + ReferenceItem.ItemAttributeValue;
                                        }
                                        else if (type == ItemType.Weapon)
                                        {
                                            tmpToDisplay = LoadItemTexture(field.X / 50, field.Y / 50, (rng.Next(1, 6) * 100) + 1, ItemType.Weapon);
                                            fields[field.X / 50, field.Y / 50].Description = ReferenceItem.Description;
                                            fields[field.X / 50, field.Y / 50].TextBuffor = "Atak: " + ReferenceItem.ItemAttributeValue;
                                        }
                                        else if (type == ItemType.Others)
                                        {
                                            tmpToDisplay = LoadItemTexture(field.X / 50, field.Y / 50, rng.Next(1, 6), ItemType.Others);
                                            fields[field.X / 50, field.Y / 50].Description = ReferenceItem.Description;
                                            if (ReferenceItem.EfType == EffectType.Healing) fields[field.X / 50, field.Y / 50].TextBuffor = "Leczenie: " + ReferenceItem.ItemAttributeValue;
                                            else if (ReferenceItem.EfType == EffectType.Learning) fields[field.X / 50, field.Y / 50].TextBuffor = "Nauka: " + ReferenceItem.ItemAttributeValue;
                                        }

                                    }
                                    if (tmpToDisplay != null)
                                    {
                                        ObjectsToRender.Add(tmpToDisplay);
                                    }
                                }
                                else if (field.Id == 5)
                                {
                                    if (Boss_Key_X == field.X / 50 && Boss_Key_Y == field.Y / 50)
                                    {
                                        tmpToDisplay = LoadMonsterTexture(field.X / 50, field.Y / 50, avatar.Level/5, true);
                                        fields[field.X / 50, field.Y / 50].Description = ReferenceMonster.Description;
                                        fields[field.X / 50, field.Y / 50].TextBuffor = "Życie: " + ReferenceMonster.CurrentHP + "/" + ReferenceMonster.MaxHP;
                                    }
                                    else
                                    {
                                        tmpToDisplay = LoadMonsterTexture(field.X / 50, field.Y / 50, (rng.Next(1, 6) * 100) + 1, false);
                                        fields[field.X / 50, field.Y / 50].Description = ReferenceMonster.Description;
                                        fields[field.X / 50, field.Y / 50].TextBuffor = "Życie: " + ReferenceMonster.CurrentHP + "/" + ReferenceMonster.MaxHP;
                                    }

                                    if (tmpToDisplay != null)
                                    {
                                        ObjectsToRender.Add(tmpToDisplay);
                                    }
                                }
                            }
                            else if (field.Explored == true)
                            {
                                if (field.Id == 5)
                                {
                                    ///walka z potworem
                                    Monster tempMonster = GeneratedMonsters.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault();
                                    if (tempMonster != null)
                                    {
                                        Arena.MonsterAP = tempMonster.Attack;
                                        Arena.MonsterDP = tempMonster.Defense;
                                        Arena.MonsterHP = tempMonster.CurrentHP;
                                        Arena.PlayerAP = avatar.Attack;
                                        Arena.PlayerDP = avatar.Defense;
                                        Arena.PlayerHP = avatar.CurrentHp;
                                        Arena.fight();

                                        Console.WriteLine(Arena.MonsterHP);
                                        tempMonster.CurrentHP = Arena.MonsterHP;
                                        avatar.CurrentHp = Arena.PlayerHP;
                                        PlayerStatusReload();
                                        if (avatar.CurrentHp <= 0)
                                        {
                                            //koniec gry przejście do końca gry i zapisanie do highscorów gracz przegrywa
                                        }
                                        else
                                        {
                                            ///pokonanie potwora
                                            if (tempMonster.CurrentHP <= 0)
                                            {
                                                //dorobić levelowanie
                                                Random rng = new Random();
                                                if (tempMonster.IsBoss == true)
                                                {
                                                    avatar.Experience += (int)(rng.Next(1, 10) * avatar.Level * 2);
                                                    avatar.Gold += (int)(rng.Next(1, 3) * avatar.Level * 2);
                                                }
                                                else
                                                {
                                                    avatar.Experience += (int)(rng.Next(1, 10) * avatar.Level / 2);
                                                    avatar.Gold += (int)(rng.Next(1, 3) * avatar.Level / 2);
                                                }
                                                PlayerStatusReload();
                                                GameObjects.Remove(GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault());
                                                GeneratedMonsters.Remove(tempMonster);
                                                InitializedObjectTexture temp = LoadFieldTexture(field.X / 50, field.Y / 50, 1, true);
                                                ObjectsToRender.Add(temp);
                                            }
                                            else
                                            {
                                                fields[field.X / 50, field.Y / 50].TextBuffor = "Życie: " + tempMonster.CurrentHP + "/" + tempMonster.MaxHP;
                                            }
                                        }
                                    }
                                }
                                else if (field.Id == 4)
                                {
                                    //podnoszenie przedmiotów
                                }
                                else if (field.Id == 3 && avatar.HasAKey == true)
                                {
                                    //następny poziom
                                }
                            }
                        }
                    }
                }

                if (gameState == GameState.InNewGame)
                {
                    if (Input.KeyPress(Key.BackSpace) && PlayersName.Length > 0)
                    {
                        this.PlayersName = this.PlayersName.Substring(0, this.PlayersName.Length - 1);
                        if (this.PlayersName.Length == 0) this.PlayersName = " ";

                        if (LastTmpText != null) TextToRender.Remove(LastTmpText);
                        TmpText = LoadTextInput(540, 190, 22, this.PlayersName);
                        LastTmpText = TmpText;
                        System.GC.Collect();
                        System.GC.WaitForPendingFinalizers();
                    }
                }
                else if (gameState == GameState.InGame)
                {
                    if (Mouse.X > 0 && Mouse.X < 500 && Mouse.Y > 0 && Mouse.Y < 500)
                    {
                        if (StatusDisplayText.Count > 0)
                        {
                            foreach (var item in StatusDisplayText.ToList())
                            {
                                TextToRender.Remove(item);
                            }      
                        }
                        var field = fields[(int)Mouse.X / 50, (int)Mouse.Y / 50];
                        StatusDisplayText.Add(LoadTextInput(510, 400, 18, field.Description));
                        if (fields[field.X / 50, field.Y / 50].TextBuffor != "")
                            StatusDisplayText.Add(LoadTextInput(510, 430, 18, fields[field.X / 50, field.Y / 50].TextBuffor));
                        System.GC.Collect();
                        System.GC.WaitForPendingFinalizers();
                    }
                }
            }
        }



        private void engine_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (gameState == GameState.InNewGame)
            {
                if ((!(Input.KeyPress(Key.BackSpace) && PlayersName.Length > 0)) && TmpText.Width < 210)
                {
                    
                    if (this.PlayersName == " ") this.PlayersName = "";
                    this.PlayersName += e.KeyChar;

                    if (LastTmpText != null) TextToRender.Remove(LastTmpText);
                    TmpText = LoadTextInput(540, 190, 22, this.PlayersName);
                    LastTmpText = TmpText;
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
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

        private void LoadToRenderGameObjects()
        {
            if (GameObjects.ToList().Count > 0)
            {
                foreach (var item in GameObjects.ToList())
                {
                    ObjectsToRender.Add(item);
                }
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
                if (ObjectsToRender.ToList().Count > 0)
                {
                    foreach (var item in ObjectsToRender.ToList())
                    {
                        ObjectsToRender.Remove(item);
                    }
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

        private void ClearGameObjectBuffer()
        {
            for (int i = 0; i < GameObjects.Count; i++)
            {
                GameObjects.Remove(GameObjects[i]);
            }
        }

        private void ClearTextRenderer()
        {
            if (TextToRender.Count > 0)
            {
                foreach (var item in TextToRender.ToList())
                {
                    TextToRender.Remove(item);
                }
            }
        }

        private void ClearTextObjectBuffer()
        {

            if (TextToRender.Count > 0)
            {
                foreach (var item in TextObjects.ToList())
                {
                    TextObjects.Remove(item);
                }
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
                    LoadText(515, 15, 44, "Politechnikon"); 
                    RenderReload();
                }        
            }
            else if (gameState == GameState.InNewGame)
            {
                if (Loading == true)
                {
                    Loading = false;
                    this.PlayersName = " ";
                    LoadBackground(0);
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
                    LoadText(515, 50, 25, "Wczytaj grę:");  
                    RenderReload();
                }
            }
            else if (gameState == GameState.InHighScore)
            {
                if (Loading == true)
                {
                    Loading = false;
                    LoadGuiTexture(500, 0, 07);
                    LoadText(515, 30, 25, "Najlepsi gracze:");  
                    RenderReload();
                }
            }
            else if (gameState == GameState.InGame)
            {
                if (Loading == true)
                {
                    Loading = false;
                    InitGame();
                    RenderReload();
                }
                else if (ReloadInterface == true)
                {
                    ReloadInterface = false;
                    ClearTextRenderer();
                    ReloadGame();
                    RenderGameStateChange();
                }
            }
            else if (gameState == GameState.AreYouSureInGame)
            {
                if (Loading == true)
                {
                    Loading = false;
                    LoadGuiTexture(500, 0, 03);
                    ClearTextRenderer();
                    ClearTextObjectBuffer();
                    LoadText(515, 190, 18, "Czy na pewno chcesz zakończyć grę?");
                    RenderGameStateChange();
                }
            }
            else if (gameState == GameState.LoadInGame)
            {
                if (Loading == true)
                {
                    Loading = false;
                    LoadGuiTexture(500, 0, 10);
                    ClearTextRenderer();
                    ClearTextObjectBuffer();
                    LoadText(515, 50, 25, "Wczytaj grę:");  
                    RenderGameStateChange();
                }
            }
            else if (gameState == GameState.NewGameInGame)
            {
                if (Loading == true)
                {
                    Loading = false;
                    LoadGuiTexture(500, 0, 03);
                    ClearTextRenderer();
                    ClearTextObjectBuffer();
                    LoadText(515, 200, 16, "Czy na pewno chcesz rozpocząć nową grę?");
                    RenderGameStateChange();
                }
            }
            else if (gameState == GameState.InSave)
            {
                if (Loading == true)
                {
                    Loading = false;
                    LoadGuiTexture(500, 0, 11);
                    ClearTextRenderer();
                    ClearTextObjectBuffer();
                    LoadText(515, 50, 25, "Zapisz grę:");
                    RenderGameStateChange();
                }
            }
        }

        private void InitGame()
        {
            this.avatar = new Avatar(this.PlayersName);
            this.fields = new Field[10,10];
            ///generowanie tła
            LoadBackground(0101);
            ///
            for (int i = 0; i < fields.GetLength(0); i++)
            {
                for (int j = 0; j < fields.GetLength(1); j++)
                {
                    /// algorytm generowania szans chancerandom((szansa jaką chcemy uzyskać/pozostałości szans(od 0.1% do 100%)) *100%)
                    if (Arena.ChanceRandom(40)) LoadFieldTexture(i, j, 1, false); ///40% szansy na wygenerowanie pola pustego
                    else if (Arena.ChanceRandom(41.66)) LoadFieldTexture(i, j, 4, false); /// 25% szansy na wygenerowanie pola z przedmiotem
                    else if (Arena.ChanceRandom(100)) LoadFieldTexture(i, j, 5, false); ///35% szansy na wygenerowanie pola z potworem
                }
            }
            //generowanie wejścia
            Random tempRand = new Random();
            int tempinX = tempRand.Next(0, 10);
            int tempinY = tempRand.Next(0, 10);
            LoadFieldTexture(tempinX, tempinY, 2, true);
            Console.WriteLine("wygenerowano poczatek na:" + tempinX + " " + tempinY);
            //generowanie wyjścia
            int tempoutX = tempRand.Next(0, 10);
            int tempoutY = tempRand.Next(0, 10);
            while (true)
            {
                if (((tempinX - tempoutX) > 2) || ((tempinY - tempoutY) > 2) || ((tempinX - tempoutX) < -2) || ((tempinY - tempoutY) < -2))
                {
                    LoadFieldTexture(tempoutX, tempoutY, 3, false);
                    Console.WriteLine("wygenerowano koniec na:" + tempoutX + " " + tempoutY);
                    break;
                }
                tempoutX = tempRand.Next(0, 10);
                tempoutY = tempRand.Next(0, 10);
            }
            //generowanie klucza
            Boss_Key_X = tempRand.Next(0, 10);
            Boss_Key_Y = tempRand.Next(0, 10);
            while (true)
			{
                if ((((tempinX - Boss_Key_X) > 2) || ((tempinY - Boss_Key_Y) > 2) || ((tempinX - Boss_Key_X) < -2) || ((tempinY - Boss_Key_Y) < -2)) &&
                    (((tempoutX - Boss_Key_X) > 2) || ((tempoutY - Boss_Key_Y) > 2) || ((tempoutX - Boss_Key_X) < -2) || ((tempoutY - Boss_Key_Y) < -2)))
				{             
                    LoadFieldTexture(Boss_Key_X, Boss_Key_Y, 4, false);               
                Console.WriteLine("wygenerowano klucz na:" + Boss_Key_X + " " + Boss_Key_Y);
				break;
				}
                Boss_Key_X = tempRand.Next(0, 10);
                Boss_Key_Y = tempRand.Next(0, 10);
			}

            LoadGuiTexture(500,0,9);
            LoadText(600-(this.PlayersName.ToList().Count * 2), 25, 14, this.PlayersName);
            LoadText(560, 45, 14, this.avatar.CurrentHp + "/" + this.avatar.MaxHp);
            LoadText(560, 68, 14, this.avatar.Defense + "");
            LoadText(560, 92, 14, this.avatar.Attack + "");
            LoadText(560, 115, 14, this.avatar.Experience + "");
            LoadText(560, 142, 14, "Brak");
            LoadText(560, 160, 16, this.avatar.Level + "");
            LoadText(560, 188, 14, this.avatar.Gold + "");
        }

        private void ReloadGame()
        {
            LoadGuiTexture(500, 0, 9);
            LoadText(600 - (this.PlayersName.ToList().Count * 2), 25, 14, this.PlayersName);
            LoadText(560, 45, 14, this.avatar.CurrentHp + "/" + this.avatar.MaxHp);
            LoadText(560, 68, 14, this.avatar.Defense + "");
            LoadText(560, 92, 14, this.avatar.Attack + "");
            LoadText(560, 115, 14, this.avatar.Experience + "");
            if (this.avatar.HasAKey) LoadText(560, 142, 14, "Zdobyty");
            else LoadText(560, 142, 14, "Brak");
            LoadText(560, 160, 16, this.avatar.Level + "");
            LoadText(560, 188, 14, this.avatar.Gold + "");
        }

        private void NextLevel()
        {

        }

        private void LoadText(int x, int y, int fontSize, String txt)
        {
            Text tekst = new Text(x, y, fontSize, txt);
            InitializedObjectTexture tekstTexture = new InitializedObjectTexture(tekst.X, tekst.Y, tekst.SizeX, tekst.SizeY, tekst.GeneratedBMP, Color.White);
            TextObjects.Add(tekstTexture);
        }

        private InitializedObjectTexture LoadTextInput(int x, int y, int fontSize, String txt, Color color)
        {
            Text tekst = new Text(x, y, fontSize, txt);
            InitializedObjectTexture tekstTexture = new InitializedObjectTexture(tekst.X, tekst.Y, tekst.SizeX, tekst.SizeY, tekst.GeneratedBMP, color);
            TextToRender.Add(tekstTexture);
            return tekstTexture;
        }

        private InitializedObjectTexture LoadTextInput(int x, int y, int fontSize, String txt)
        {
            Text tekst = new Text(x, y, fontSize, txt);
            InitializedObjectTexture tekstTexture = new InitializedObjectTexture(tekst.X, tekst.Y, tekst.SizeX, tekst.SizeY, tekst.GeneratedBMP, Color.White);
            TextToRender.Add(tekstTexture);
            return tekstTexture;
        }

        private InitializedObjectTexture LoadFieldTexture(int x, int y, int id, bool explored)
        {
            fields[x, y] = new Field(x * 50, y * 50, id, explored);
            InitializedObjectTexture FieldTexture = new InitializedObjectTexture(fields[x, y].X, fields[x, y].Y, fields[x, y].SizeX, fields[x, y].SizeY, fields[x, y].Path, Color.White);
            GameObjects.Add(FieldTexture);
            return FieldTexture;
        }

        private InitializedObjectTexture LoadItemTexture(int x, int y, int id, ItemType type)
        {
            Item item = new Item(x * 50, y * 50, id, type);
            ReferenceItem = item;
            GeneratedItems.Add(item);
            InitializedObjectTexture ItemTexture = new InitializedObjectTexture(item.X, item.Y, item.SizeX, item.SizeY, item.Path, Color.White);
            GameObjects.Add(ItemTexture);
            return ItemTexture;
        }

        private InitializedObjectTexture LoadMonsterTexture(int x, int y, int id, bool boss)
        {
            Monster monster = new Monster(x * 50, y * 50, id, boss);
            ReferenceMonster = monster;
            GeneratedMonsters.Add(monster);
            InitializedObjectTexture monsterTexture = new InitializedObjectTexture(monster.X, monster.Y, monster.SizeX, monster.SizeY, monster.Path, Color.White);
            GameObjects.Add(monsterTexture);
            return monsterTexture;
        }

        private void LoadGuiTexture(int x, int y, int id)
        {
            Background background = new Background(x, y, id, BackgroundType.GUI);
            InitializedObjectTexture Menu = new InitializedObjectTexture(background.X, background.Y, background.SizeX, background.SizeY, background.Path, Color.White);
            InterfaceObjects.Add(Menu);
        }

        private void LoadBackground(int id)
        {
            Background Background = new Background(0, 0, id, BackgroundType.Board);
            InitializedObjectTexture BackgroundTexture = new InitializedObjectTexture(Background.X, Background.Y, Background.SizeX, Background.SizeY, Background.Path, Color.White);
            GameObjects.Add(BackgroundTexture);
        }

        private void PlayerStatusReload()
        {
            LoadText(600 - (this.PlayersName.ToList().Count * 2), 25, 14, this.PlayersName);
            LoadText(560, 45, 14, this.avatar.CurrentHp + "/" + this.avatar.MaxHp);
            LoadText(560, 68, 14, this.avatar.Defense + "");
            LoadText(560, 92, 14, this.avatar.Attack + "");
            LoadText(560, 115, 14, this.avatar.Experience + "");
            LoadText(560, 142, 14, "Brak");
            LoadText(560, 160, 16, this.avatar.Level + "");
            LoadText(560, 188, 14, this.avatar.Gold + "");
            TextRenderReload();
        }

        private void RenderReload()
        {
            if (LastTmpText != null) TextToRender.Remove(LastTmpText);
            if (ObjectsToRemove.ToList().Count > 0)
            {
                foreach (var item in ObjectsToRemove.ToList())
                {
                    if (TextToRender.Contains(item)) TextToRender.Remove(item);
                    if (ObjectsToRender.Contains(item)) ObjectsToRender.Remove(item);
                }
                foreach (var item in ObjectsToRemove.ToList())
                {
                    ObjectsToRemove.Remove(item);
                }
            }
            ClearRenderer(BufforClear.Everything);
            ClearTextRenderer();
            LoadToRenderInterfaceObjects();
            LoadToRenderGameObjects();
            LoadToRenderTextObjects();
            ClearInterfaceObjectBuffer();
            ClearTextObjectBuffer();
            ClearGameObjectBuffer();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        private void RenderGameStateChange()
        {
            ClearRenderer(BufforClear.InterfaceObjects);
            LoadToRenderInterfaceObjects();
            LoadToRenderTextObjects();
            ClearInterfaceObjectBuffer();
            ClearTextObjectBuffer();
        }

        private void TextRenderReload()
        {
            ClearTextRenderer();
            LoadToRenderTextObjects();
            ClearTextObjectBuffer();
        }

    }
}
