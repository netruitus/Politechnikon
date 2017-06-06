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
using Microsoft.Win32;
using System.Timers;
using System.IO;





namespace Politechnikon.game_logic
{
    public struct Score
    {
        private String name;
        private int exp;
        private bool thisgame;

        public String Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        public int Exp
        {
            get { return this.exp; }
            set { this.exp = value; }
        }

        public bool ThisGame
        {
            get { return this.thisgame; }
            set { this.thisgame = value; }
        }
    }

    public enum EquipmentElementUsed
    {
        Potion,
        Weared,
        Backpack,
        Nothing
    }

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
        EndGame
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
        private List<InitializedObjectTexture> StatusDisplayText;
        private List<InitializedObjectTexture> TemporaryInitEqList;
        private InitializedObjectTexture[] EquipmentObjects;
        private List<Item> GeneratedItems;
        private List<Monster> GeneratedMonsters;
        private List<Text> Texts;
        private InitializedObjectTexture LastTmpText;
        private InitializedObjectTexture TmpText;
        private InitializedObjectTexture BossAlert;
        private InitializedObjectTexture DialogWindow;
        private InitializedObjectTexture CheckingItemTexture;
        private GameState gameState;
        private GameState lastGameState;
        private Avatar avatar;
        private Field[,] fields;
        private Timer timer;
        private OpenTK.Input.MouseDevice Mouse;
        private Engine engine;
        private String PlayersName;
        private bool Loading;
        private bool QuitConfirm;
        private bool ReloadInterface;
        private bool WonTheGame;
        private bool GamePlayed;
        private bool LoadingDialog;
        private EquipmentElementUsed ElementUsed;
        private Item ReferenceItem;
        private Item CheckedItem;
        private Monster ReferenceMonster;
        private int Boss_Key_X;
        private int Boss_Key_Y;
        private int Dungeon_Level;
        private int Item_Enchant;
        private int Avatars_Comparison_Level;
        
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
            ///inicjalizacja obiektów i zmiennych
            this.ReloadInterface = false;
            this.EquipmentObjects = new InitializedObjectTexture[14]; /// 14 slotów w ekwipunku - 0 - 7 plecak, 8 - 9 mikstury, 10 - 13 - ekwipunek założony
            this.InterfaceObjects = new List<InitializedObjectTexture>();
            this.TextObjects = new List<InitializedObjectTexture>();
            this.ObjectsToRemove = new List<InitializedObjectTexture>();
            this.GameObjects = new List<InitializedObjectTexture>();
            this.StatusDisplayText = new List<InitializedObjectTexture>();
            this.TemporaryInitEqList = new List<InitializedObjectTexture>();
            this.GeneratedItems = new List<Item>();
            this.GeneratedMonsters = new List<Monster>();
            this.Texts = new List<Text>();
            this.GamePlayed = false;
            PlayersName = "";
            TmpText = LoadTextInput(1, 1, 3, " ");
            BossAlert = LoadTextInput(1, 1, 3, " ");
        }

        public void GetInput()
        {
        ///zbieranie inputów myszy -- głównie zmiany stanów
            if (Loading == false)
            {
                ///kliknięcie lewym przyciskiem myszy
                if (Input.MousePress(OpenTK.Input.MouseButton.Left))
                {
                    ///stan gry w menu
                    if (gameState == GameState.InMenu)
                    {
                        ///kliknięcie zakończ
                        if (Mouse.X > 562 && Mouse.X < 737 && Mouse.Y > 329 && Mouse.Y < 368)
                        {
                            QuitConfirm = true;
                            lastGameState = gameState;
                            gameState = GameState.AreYouSure;
                            Loading = true;   
                        }
                        ///kliknięcie w najwyższe wyniki
                        else if (Mouse.X > 562 && Mouse.X < 737 && Mouse.Y > 258 && Mouse.Y < 298)
                        {
                            lastGameState = gameState;
                            gameState = GameState.InHighScore;
                            Loading = true;
                        }
                        ///kliknięcie w wczytaj grę
                        else if (Mouse.X > 562 && Mouse.X < 737 && Mouse.Y > 186 && Mouse.Y < 225)
                        {
                            lastGameState = gameState;
                            gameState = GameState.InLoad;
                            Loading = true;
                        }
                        ///kliknięcie w nową grę
                        else if (Mouse.X > 562 && Mouse.X < 737 && Mouse.Y > 115 && Mouse.Y < 155)
                        {
                            lastGameState = gameState;
                            gameState = GameState.InNewGame;
                            Loading = true;
                        }
                    }
                    ///stan gry w oknie nowej gry
                    else if (gameState == GameState.InNewGame)
                    {
                        ///kliknięcie anuluj
                        if (Mouse.X > 656 && Mouse.X < 744 && Mouse.Y > 251 && Mouse.Y < 282)
                        {
                            gameState = GameState.InMenu;
                            lastGameState = GameState.InNewGame;
                            Loading = true;
                        }
                        ///kliknięcie zatwierdź
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
                       ///kliknięcie zakończ
                        else if (Mouse.X > 723 && Mouse.X < 793 && Mouse.Y > 4 && Mouse.Y < 28)
                        {
                            QuitConfirm = true;
                            lastGameState = gameState;
                            gameState = GameState.AreYouSure;
                            Loading = true; 
                        }
                        ///kliknięcie wczytaj
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
                    }
                    else if (gameState == GameState.EndGame)
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
                        ///kliknięcie zakończ
                        if (Mouse.X > 723 && Mouse.X < 793 && Mouse.Y > 4 && Mouse.Y < 28)
                        {
                            QuitConfirm = true;
                            lastGameState = gameState;
                            gameState = GameState.AreYouSureInGame;
                            Loading = true;
                        }
                        ///kliknięcie wczytaj
                        else if (Mouse.X > 651 && Mouse.X < 720 && Mouse.Y > 4 && Mouse.Y < 28)
                        {
                            lastGameState = gameState;
                            gameState = GameState.LoadInGame;
                            Loading = true;
                        }
                        ///kliknięcie zapisz
                        else if (Mouse.X > 520 && Mouse.X < 595 && Mouse.Y > 445 && Mouse.Y < 476)
                        {

                        }
                        ///kliknięcie usuń
                        else if (Mouse.X > 603 && Mouse.X < 679 && Mouse.Y > 445 && Mouse.Y < 476)
                        {

                        }
                        ///kliknięcie anuluj
                        else if (Mouse.X > 685 && Mouse.X < 760 && Mouse.Y > 445 && Mouse.Y < 476)
                        {
                            gameState = GameState.InGame;
                            lastGameState = GameState.InSave;
                            ReloadInterface = true;
                        }
                        ///kliknięcie górnej strzałki
                        else if (Mouse.X > 758 && Mouse.X < 773 && Mouse.Y > 264 && Mouse.Y < 313)
                        {

                        }
                        ///kliknięcie dolnej strzałki
                        else if (Mouse.X > 758 && Mouse.X < 773 && Mouse.Y > 313 && Mouse.Y < 364)
                        {

                        }
                    }
                    ///stan gry - w grze
                    else if (gameState == GameState.InGame)
                    {
                        ///kliknięcie zakończ
                        if (Mouse.X > 723 && Mouse.X < 793 && Mouse.Y > 4 && Mouse.Y < 28)
                        {
                            QuitConfirm = true;
                            lastGameState = gameState;
                            gameState = GameState.AreYouSureInGame;
                            Loading = true;
                        }
                        ///kliknięcie nowa gra
                        else if (Mouse.X > 505 && Mouse.X < 577 && Mouse.Y > 4 && Mouse.Y < 28)
                        {
                            lastGameState = gameState;
                            gameState = GameState.NewGameInGame;
                            Loading = true;
                        }
                        ///kliknięcie wczytaj
                        else if (Mouse.X > 651 && Mouse.X < 720 && Mouse.Y > 4 && Mouse.Y < 28)
                        {
                            lastGameState = gameState;
                            gameState = GameState.LoadInGame;
                            Loading = true;
                        }
                        ///kliknięcie zapisz grę
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
                            if (field.Explored == false && isExploredNearby(field.X, field.Y) && !(isMonsterNearby(field.X, field.Y))) 
                            {
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
                                            tmpToDisplay = LoadItemTexture(field.X / 50, field.Y / 50, (rng.Next(1, 6) * 100) + Item_Enchant, ItemType.Armor);
                                            fields[field.X / 50, field.Y / 50].Description = ReferenceItem.Description;
                                            fields[field.X / 50, field.Y / 50].TextBuffor = "Obrona: " + ReferenceItem.ItemAttributeValue;
                                        }
                                        else if (type == ItemType.Weapon)
                                        {
                                            tmpToDisplay = LoadItemTexture(field.X / 50, field.Y / 50, (rng.Next(1, 6) * 100) + Item_Enchant, ItemType.Weapon);
                                            fields[field.X / 50, field.Y / 50].Description = ReferenceItem.Description;
                                            fields[field.X / 50, field.Y / 50].TextBuffor = "Atak: " + ReferenceItem.ItemAttributeValue;
                                        }
                                        else if (type == ItemType.Others)
                                        {
                                            tmpToDisplay = LoadItemTexture(field.X / 50, field.Y / 50, rng.Next(1, 6), ItemType.Others);
                                            fields[field.X / 50, field.Y / 50].Description = ReferenceItem.Description;
                                            if (ReferenceItem.EfType == EffectType.Healing)
                                            {
                                                ReferenceItem.ItemAttributeValue = (int)(ReferenceItem.ItemAttributeValue * (1 + 0.05 * (avatar.Level - 1)));
                                                fields[field.X / 50, field.Y / 50].TextBuffor = "Leczenie: " + ReferenceItem.ItemAttributeValue;
                                            }
                                            else if (ReferenceItem.EfType == EffectType.Learning)
                                            {
                                                ReferenceItem.ItemAttributeValue = (int)(ReferenceItem.ItemAttributeValue * (1 + 0.3 * (Dungeon_Level - 1)));
                                                fields[field.X / 50, field.Y / 50].TextBuffor = "Nauka: " + ReferenceItem.ItemAttributeValue;
                                            }
                                        }

                                    }
                                    if (tmpToDisplay != null)
                                    {
                                        ObjectsToRender.Add(tmpToDisplay);
                                    }
                                }
                                else if (field.Id == 5)
                                {
                                    if (Boss_Key_X == field.X / 50 && Boss_Key_Y == field.Y / 50 && Dungeon_Level%5 == 0)
                                    {
                                        tmpToDisplay = LoadMonsterTexture(field.X / 50, field.Y / 50, Dungeon_Level/5, true);
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

                                        tempMonster.CurrentHP = Arena.MonsterHP;
                                        avatar.CurrentHp = Arena.PlayerHP;
                                        PlayerStatusReload();
                                        if (avatar.CurrentHp <= 0)
                                        {
                                            lastGameState = gameState;
                                            gameState = GameState.EndGame;
                                            Loading = true;
                                        }
                                        else
                                        {
                                            ///pokonanie potwora
                                            if (tempMonster.CurrentHP <= 0)
                                            {
                                                avatar.Experience += tempMonster.Experience;
                                                PlayerStatusReload();
                                                InitializedObjectTexture temp;
                                                InitializedObjectTexture tmpToDisplay = null;
                                                if (tempMonster.IsBoss == true)
                                                {
                                                    temp = LoadFieldTexture(field.X / 50, field.Y / 50, 4, true);
                                                    tmpToDisplay = LoadItemTexture(field.X / 50, field.Y / 50, 0, ItemType.Others);
                                                    fields[field.X / 50, field.Y / 50].Description = ReferenceItem.Description;
                                                }
                                                else
                                                {
                                                    temp = LoadFieldTexture(field.X / 50, field.Y / 50, 1, true);
                                                }
                                                if (GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault() != null)
                                                    GameObjects.Remove(GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault());
                                                GeneratedMonsters.Remove(tempMonster);
                                                ObjectsToRender.Add(temp);
                                                if (tmpToDisplay != null) ObjectsToRender.Add(tmpToDisplay);
                                                IncreaseHealingEfficiety();
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
                                    ///podnoszenie przedmiotów
                                    Item tempItem = GeneratedItems.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault();
                                    ///podniesienie klucza
                                    if (tempItem.Id == 0)
                                    {
                                        if (GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault() != null)
                                        GameObjects.Remove(GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault());
                                        GeneratedItems.Remove(tempItem);
                                        this.avatar.HasAKey = true;
                                        InitializedObjectTexture temp = LoadFieldTexture(field.X / 50, field.Y / 50, 1, true);
                                        ObjectsToRender.Add(temp);
                                        PlayerStatusReload();
                                    }
                                    ///podniesienie przedmiotu dającego XP
                                    else if (tempItem.EfType == EffectType.Learning)
                                    {
                                        if (GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault() != null)
                                        GameObjects.Remove(GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault());
                                        GeneratedItems.Remove(tempItem);
                                        avatar.Experience += tempItem.ItemAttributeValue;
                                        InitializedObjectTexture temp = LoadFieldTexture(field.X / 50, field.Y / 50, 1, true);
                                        ObjectsToRender.Add(temp);
                                        PlayerStatusReload();
                                        IncreaseHealingEfficiety();
                                    }
                                    ///podniesienie potki
                                    else if (tempItem.EfType == EffectType.Healing && tempItem.Id == 1)
                                    {
                                        if (avatar.Potions[0].Id == 255)
                                        {
                                            int X = avatar.Potions[0].X;
                                            int Y = avatar.Potions[0].Y;
                                            avatar.Potions[0] = tempItem;
                                            if (GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault() != null)
                                            GameObjects.Remove(GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault());
                                            GeneratedItems.Remove(tempItem);
                                            InitializedObjectTexture temp = LoadFieldTexture(field.X / 50, field.Y / 50, 1, true);
                                            ObjectsToRender.Add(temp);
                                            avatar.Potions[0].X = X;
                                            avatar.Potions[0].Y = Y;
                                            RenderGameStateChange();
                                        }
                                        else if (avatar.Potions[1].Id == 255)
                                        {
                                            int X = avatar.Potions[1].X;
                                            int Y = avatar.Potions[1].Y;
                                            avatar.Potions[1] = tempItem;
                                            if (GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault() != null)
                                            GameObjects.Remove(GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault());
                                            GeneratedItems.Remove(tempItem);
                                            InitializedObjectTexture temp = LoadFieldTexture(field.X / 50, field.Y / 50, 1, true);
                                            ObjectsToRender.Add(temp);
                                            avatar.Potions[1].X = X;
                                            avatar.Potions[1].Y = Y;
                                            RenderGameStateChange();
                                        } 
                                    }
                                    ///podniesienie pozostałych rzeczy
                                    else
                                    {
                                        if (avatar.Backpack[0].Id == 255)
                                        {
                                            avatar.Backpack[0] = new Item(avatar.Backpack[0].X, avatar.Backpack[0].Y, tempItem.Id, tempItem.ItType);
                                            if (GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault() != null)
                                            GameObjects.Remove(GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault());
                                            GeneratedItems.Remove(tempItem);
                                            InitializedObjectTexture temp = LoadFieldTexture(field.X / 50, field.Y / 50, 1, true);
                                            ObjectsToRender.Add(temp);
                                            RenderGameStateChange();
                                        }
                                        else if (avatar.Backpack[1].Id == 255)
                                        {
                                            avatar.Backpack[1] = new Item(avatar.Backpack[1].X, avatar.Backpack[1].Y, tempItem.Id, tempItem.ItType);
                                            if (GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault() != null)
                                            GameObjects.Remove(GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault());
                                            GeneratedItems.Remove(tempItem);
                                            InitializedObjectTexture temp = LoadFieldTexture(field.X / 50, field.Y / 50, 1, true);
                                            ObjectsToRender.Add(temp);
                                            RenderGameStateChange();
                                        }
                                        else if (avatar.Backpack[2].Id == 255)
                                        {
                                            avatar.Backpack[2] = new Item(avatar.Backpack[2].X, avatar.Backpack[2].Y, tempItem.Id, tempItem.ItType);
                                            if (GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault() != null)
                                            GameObjects.Remove(GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault());
                                            GeneratedItems.Remove(tempItem);
                                            InitializedObjectTexture temp = LoadFieldTexture(field.X / 50, field.Y / 50, 1, true);
                                            ObjectsToRender.Add(temp);
                                            RenderGameStateChange();
                                        }
                                        else if (avatar.Backpack[3].Id == 255)
                                        {
                                            avatar.Backpack[3] = new Item(avatar.Backpack[3].X, avatar.Backpack[3].Y, tempItem.Id, tempItem.ItType);
                                            if (GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault() != null)
                                            GameObjects.Remove(GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault());
                                            GeneratedItems.Remove(tempItem);
                                            InitializedObjectTexture temp = LoadFieldTexture(field.X / 50, field.Y / 50, 1, true);
                                            ObjectsToRender.Add(temp);
                                            RenderGameStateChange();
                                        }
                                        else if (avatar.Backpack[4].Id == 255)
                                        {
                                            avatar.Backpack[4] = new Item(avatar.Backpack[4].X, avatar.Backpack[4].Y, tempItem.Id, tempItem.ItType);
                                            if (GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault() != null)
                                            GameObjects.Remove(GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault());
                                            GeneratedItems.Remove(tempItem);
                                            InitializedObjectTexture temp = LoadFieldTexture(field.X / 50, field.Y / 50, 1, true);
                                            ObjectsToRender.Add(temp);
                                            RenderGameStateChange();
                                        }
                                        else if (avatar.Backpack[5].Id == 255)
                                        {
                                            avatar.Backpack[5] = new Item(avatar.Backpack[5].X, avatar.Backpack[5].Y, tempItem.Id, tempItem.ItType);
                                            if (GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault() != null)
                                            GameObjects.Remove(GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault());
                                            GeneratedItems.Remove(tempItem);
                                            InitializedObjectTexture temp = LoadFieldTexture(field.X / 50, field.Y / 50, 1, true);
                                            ObjectsToRender.Add(temp);
                                            RenderGameStateChange();
                                        }
                                        else if (avatar.Backpack[6].Id == 255)
                                        {
                                            avatar.Backpack[6] = new Item(avatar.Backpack[6].X, avatar.Backpack[6].Y, tempItem.Id, tempItem.ItType);
                                            if (GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault() != null)
                                            GameObjects.Remove(GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault());
                                            GeneratedItems.Remove(tempItem);
                                            InitializedObjectTexture temp = LoadFieldTexture(field.X / 50, field.Y / 50, 1, true);
                                            ObjectsToRender.Add(temp);
                                            RenderGameStateChange();
                                        }
                                        else if (avatar.Backpack[7].Id == 255)
                                        {
                                            avatar.Backpack[7] = new Item(avatar.Backpack[7].X, avatar.Backpack[7].Y, tempItem.Id, tempItem.ItType);
                                            if (GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault() != null)
                                            GameObjects.Remove(GameObjects.Where(x => x.X == field.X && x.Y == field.Y).FirstOrDefault());
                                            GeneratedItems.Remove(tempItem);
                                            InitializedObjectTexture temp = LoadFieldTexture(field.X / 50, field.Y / 50, 1, true);
                                            ObjectsToRender.Add(temp);
                                            RenderGameStateChange();
                                        }
                                    }
                                    
                                }
                                else if (field.Id == 3 && avatar.HasAKey == true)
                                {
                                    ///przejście do następnego poziomu
                                    InitNextLevel();
                                }
                            }
                        }
                        else if (Mouse.X > 500 && Mouse.X < 800 && Mouse.Y > 0 && Mouse.Y < 500)
                        {
                            ///panel z wyborem
                            if (Mouse.X > 650 && Mouse.X < 710 && Mouse.Y > 180 && Mouse.Y < 200)
                            {
                                
                                if (CheckingItemTexture != null) ObjectsToRender.Remove(CheckingItemTexture);
                                if (DialogWindow != null) ObjectsToRender.Remove(DialogWindow);
                                ///zakładanie/zdejmowanie/używanie przedmiotów
                                if (ElementUsed == EquipmentElementUsed.Potion)
                                {
                                    ElementUsed = EquipmentElementUsed.Nothing;
                                    if (CheckedItem.Equals(avatar.Potions[0]) && avatar.CurrentHp != avatar.MaxHp)
                                    {
                                        avatar.CurrentHp += avatar.Potions[0].ItemAttributeValue;
                                        avatar.Potions[0] = new Item(avatar.Potions[0].X, avatar.Potions[0].Y, 255, ItemType.Others);              
                                    }
                                    else if (CheckedItem.Equals(avatar.Potions[1]) && avatar.CurrentHp != avatar.MaxHp)
                                    {
                                        avatar.CurrentHp += avatar.Potions[1].ItemAttributeValue;
                                        avatar.Potions[1] = new Item(avatar.Potions[1].X, avatar.Potions[1].Y, 255, ItemType.Others);         
                                    }
                                    PlayerStatusReload();
                                    ReloadRenderEq();
                                }
                                else if (ElementUsed == EquipmentElementUsed.Backpack)
                                {
                                    ElementUsed = EquipmentElementUsed.Nothing;
                                    if (CheckedItem.Equals(avatar.Backpack[0]))
                                    {
                                        if (avatar.Backpack[0].ItType == ItemType.Others && avatar.Backpack[0].EfType == EffectType.Healing && avatar.CurrentHp != avatar.MaxHp)
                                        {
                                            avatar.CurrentHp += avatar.Backpack[0].ItemAttributeValue;
                                            avatar.Backpack[0] = new Item(avatar.Backpack[0].X, avatar.Backpack[0].Y, 255, ItemType.Others);                                  
                                        }
                                        else
                                        {
                                            if (avatar.Equipment[0].Id == 255 && avatar.Backpack[0].ItType == ItemType.Weapon)
                                            {
                                                avatar.Attack += avatar.Backpack[0].ItemAttributeValue;
                                                avatar.Equipment[0] = new Item(avatar.Equipment[0].X, avatar.Equipment[0].Y, avatar.Backpack[0].Id, avatar.Backpack[0].ItType);
                                                avatar.Backpack[0] = new Item(avatar.Backpack[0].X, avatar.Backpack[0].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[1].Id == 255 && avatar.Backpack[0].ItType == ItemType.Weapon) 
                                            {
                                                avatar.Attack += avatar.Backpack[0].ItemAttributeValue;
                                                avatar.Equipment[1] = new Item(avatar.Equipment[1].X, avatar.Equipment[1].Y, avatar.Backpack[0].Id, avatar.Backpack[0].ItType);
                                                avatar.Backpack[0] = new Item(avatar.Backpack[0].X, avatar.Backpack[0].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[2].Id == 255 && avatar.Backpack[0].ItType == ItemType.Armor)
                                            {
                                                avatar.Defense += avatar.Backpack[0].ItemAttributeValue;
                                                avatar.Equipment[2] = new Item(avatar.Equipment[2].X, avatar.Equipment[2].Y, avatar.Backpack[0].Id, avatar.Backpack[0].ItType);
                                                avatar.Backpack[0] = new Item(avatar.Backpack[0].X, avatar.Backpack[0].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[3].Id == 255 && avatar.Backpack[0].ItType == ItemType.Armor)
                                            {
                                                avatar.Defense += avatar.Backpack[0].ItemAttributeValue;
                                                avatar.Equipment[3] = new Item(avatar.Equipment[3].X, avatar.Equipment[3].Y, avatar.Backpack[0].Id, avatar.Backpack[0].ItType);
                                                avatar.Backpack[0] = new Item(avatar.Backpack[0].X, avatar.Backpack[0].Y, 255, ItemType.Others);
                                            }    
                                        }  
                                    }
                                    else if (CheckedItem.Equals(avatar.Backpack[1]))
                                    {
                                        if (avatar.Backpack[1].ItType == ItemType.Others && avatar.Backpack[1].EfType == EffectType.Healing && avatar.CurrentHp != avatar.MaxHp)
                                        {
                                            avatar.CurrentHp += avatar.Backpack[1].ItemAttributeValue;
                                            avatar.Backpack[1] = new Item(avatar.Backpack[1].X, avatar.Backpack[1].Y, 255, ItemType.Others);
                                        }
                                        else
                                        {
                                            if (avatar.Equipment[0].Id == 255 && avatar.Backpack[1].ItType == ItemType.Weapon)
                                            {
                                                avatar.Attack += avatar.Backpack[1].ItemAttributeValue;
                                                avatar.Equipment[0] = new Item(avatar.Equipment[0].X, avatar.Equipment[0].Y, avatar.Backpack[1].Id, avatar.Backpack[1].ItType);
                                                avatar.Backpack[1] = new Item(avatar.Backpack[1].X, avatar.Backpack[1].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[1].Id == 255 && avatar.Backpack[1].ItType == ItemType.Weapon)
                                            {
                                                avatar.Attack += avatar.Backpack[1].ItemAttributeValue;
                                                avatar.Equipment[1] = new Item(avatar.Equipment[1].X, avatar.Equipment[1].Y, avatar.Backpack[1].Id, avatar.Backpack[1].ItType);
                                                avatar.Backpack[1] = new Item(avatar.Backpack[1].X, avatar.Backpack[1].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[2].Id == 255 && avatar.Backpack[1].ItType == ItemType.Armor)
                                            {
                                                avatar.Defense += avatar.Backpack[1].ItemAttributeValue;
                                                avatar.Equipment[2] = new Item(avatar.Equipment[2].X, avatar.Equipment[2].Y, avatar.Backpack[1].Id, avatar.Backpack[1].ItType);
                                                avatar.Backpack[1] = new Item(avatar.Backpack[1].X, avatar.Backpack[1].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[3].Id == 255 && avatar.Backpack[1].ItType == ItemType.Armor)
                                            {
                                                avatar.Defense += avatar.Backpack[1].ItemAttributeValue;
                                                avatar.Equipment[3] = new Item(avatar.Equipment[3].X, avatar.Equipment[3].Y, avatar.Backpack[1].Id, avatar.Backpack[1].ItType);
                                                avatar.Backpack[1] = new Item(avatar.Backpack[1].X, avatar.Backpack[1].Y, 255, ItemType.Others);
                                            }
                                        }  
                                    }
                                    else if (CheckedItem.Equals(avatar.Backpack[2]))
                                    {
                                        if (avatar.Backpack[2].ItType == ItemType.Others && avatar.Backpack[2].EfType == EffectType.Healing && avatar.CurrentHp != avatar.MaxHp)
                                        {
                                            avatar.CurrentHp += avatar.Backpack[2].ItemAttributeValue;
                                            avatar.Backpack[2] = new Item(avatar.Backpack[2].X, avatar.Backpack[2].Y, 255, ItemType.Others);
                                        }
                                        else
                                        {
                                            if (avatar.Equipment[0].Id == 255 && avatar.Backpack[2].ItType == ItemType.Weapon)
                                            {
                                                avatar.Attack += avatar.Backpack[2].ItemAttributeValue;
                                                avatar.Equipment[0] = new Item(avatar.Equipment[0].X, avatar.Equipment[0].Y, avatar.Backpack[2].Id, avatar.Backpack[2].ItType);
                                                avatar.Backpack[2] = new Item(avatar.Backpack[2].X, avatar.Backpack[2].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[1].Id == 255 && avatar.Backpack[2].ItType == ItemType.Weapon)
                                            {
                                                avatar.Attack += avatar.Backpack[2].ItemAttributeValue;
                                                avatar.Equipment[1] = new Item(avatar.Equipment[1].X, avatar.Equipment[1].Y, avatar.Backpack[2].Id, avatar.Backpack[2].ItType);
                                                avatar.Backpack[2] = new Item(avatar.Backpack[2].X, avatar.Backpack[2].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[2].Id == 255 && avatar.Backpack[2].ItType == ItemType.Armor)
                                            {
                                                avatar.Defense += avatar.Backpack[2].ItemAttributeValue;
                                                avatar.Equipment[2] = new Item(avatar.Equipment[2].X, avatar.Equipment[2].Y, avatar.Backpack[2].Id, avatar.Backpack[2].ItType);
                                                avatar.Backpack[2] = new Item(avatar.Backpack[2].X, avatar.Backpack[2].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[3].Id == 255 && avatar.Backpack[2].ItType == ItemType.Armor)
                                            {
                                                avatar.Defense += avatar.Backpack[2].ItemAttributeValue;
                                                avatar.Equipment[3] = new Item(avatar.Equipment[3].X, avatar.Equipment[3].Y, avatar.Backpack[2].Id, avatar.Backpack[2].ItType);
                                                avatar.Backpack[2] = new Item(avatar.Backpack[2].X, avatar.Backpack[2].Y, 255, ItemType.Others);
                                            }
                                        }
                                    }
                                    else if (CheckedItem.Equals(avatar.Backpack[3]))
                                    {
                                        if (avatar.Backpack[3].ItType == ItemType.Others && avatar.Backpack[3].EfType == EffectType.Healing && avatar.CurrentHp != avatar.MaxHp)
                                        {
                                            avatar.CurrentHp += avatar.Backpack[3].ItemAttributeValue;
                                            avatar.Backpack[3] = new Item(avatar.Backpack[3].X, avatar.Backpack[3].Y, 255, ItemType.Others);
                                        }
                                        else
                                        {
                                            if (avatar.Equipment[0].Id == 255 && avatar.Backpack[3].ItType == ItemType.Weapon)
                                            {
                                                avatar.Attack += avatar.Backpack[3].ItemAttributeValue;
                                                avatar.Equipment[0] = new Item(avatar.Equipment[0].X, avatar.Equipment[0].Y, avatar.Backpack[3].Id, avatar.Backpack[3].ItType);
                                                avatar.Backpack[3] = new Item(avatar.Backpack[3].X, avatar.Backpack[3].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[1].Id == 255 && avatar.Backpack[3].ItType == ItemType.Weapon)
                                            {
                                                avatar.Attack += avatar.Backpack[3].ItemAttributeValue;
                                                avatar.Equipment[1] = new Item(avatar.Equipment[1].X, avatar.Equipment[1].Y, avatar.Backpack[3].Id, avatar.Backpack[3].ItType);
                                                avatar.Backpack[3] = new Item(avatar.Backpack[3].X, avatar.Backpack[3].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[2].Id == 255 && avatar.Backpack[3].ItType == ItemType.Armor)
                                            {
                                                avatar.Defense += avatar.Backpack[3].ItemAttributeValue;
                                                avatar.Equipment[2] = new Item(avatar.Equipment[2].X, avatar.Equipment[2].Y, avatar.Backpack[3].Id, avatar.Backpack[3].ItType);
                                                avatar.Backpack[3] = new Item(avatar.Backpack[3].X, avatar.Backpack[3].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[3].Id == 255 && avatar.Backpack[3].ItType == ItemType.Armor)
                                            {
                                                avatar.Defense += avatar.Backpack[3].ItemAttributeValue;
                                                avatar.Equipment[3] = new Item(avatar.Equipment[3].X, avatar.Equipment[3].Y, avatar.Backpack[3].Id, avatar.Backpack[3].ItType);
                                                avatar.Backpack[3] = new Item(avatar.Backpack[3].X, avatar.Backpack[3].Y, 255, ItemType.Others);
                                            }
                                        }
                                    }
                                    else if (CheckedItem.Equals(avatar.Backpack[4]))
                                    {
                                        if (avatar.Backpack[4].ItType == ItemType.Others && avatar.Backpack[4].EfType == EffectType.Healing && avatar.CurrentHp != avatar.MaxHp)
                                        {
                                            avatar.CurrentHp += avatar.Backpack[4].ItemAttributeValue;
                                            avatar.Backpack[4] = new Item(avatar.Backpack[4].X, avatar.Backpack[4].Y, 255, ItemType.Others);
                                        }
                                        else
                                        {
                                            if (avatar.Equipment[0].Id == 255 && avatar.Backpack[4].ItType == ItemType.Weapon)
                                            {
                                                avatar.Attack += avatar.Backpack[4].ItemAttributeValue;
                                                avatar.Equipment[0] = new Item(avatar.Equipment[0].X, avatar.Equipment[0].Y, avatar.Backpack[4].Id, avatar.Backpack[4].ItType);
                                                avatar.Backpack[4] = new Item(avatar.Backpack[4].X, avatar.Backpack[4].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[1].Id == 255 && avatar.Backpack[4].ItType == ItemType.Weapon)
                                            {
                                                avatar.Attack += avatar.Backpack[4].ItemAttributeValue;
                                                avatar.Equipment[1] = new Item(avatar.Equipment[1].X, avatar.Equipment[1].Y, avatar.Backpack[4].Id, avatar.Backpack[4].ItType);
                                                avatar.Backpack[4] = new Item(avatar.Backpack[4].X, avatar.Backpack[4].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[2].Id == 255 && avatar.Backpack[4].ItType == ItemType.Armor)
                                            {
                                                avatar.Defense += avatar.Backpack[4].ItemAttributeValue;
                                                avatar.Equipment[2] = new Item(avatar.Equipment[2].X, avatar.Equipment[2].Y, avatar.Backpack[4].Id, avatar.Backpack[4].ItType);
                                                avatar.Backpack[4] = new Item(avatar.Backpack[4].X, avatar.Backpack[4].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[3].Id == 255 && avatar.Backpack[4].ItType == ItemType.Armor)
                                            {
                                                avatar.Defense += avatar.Backpack[4].ItemAttributeValue;
                                                avatar.Equipment[3] = new Item(avatar.Equipment[3].X, avatar.Equipment[3].Y, avatar.Backpack[4].Id, avatar.Backpack[4].ItType);
                                                avatar.Backpack[4] = new Item(avatar.Backpack[4].X, avatar.Backpack[4].Y, 255, ItemType.Others);
                                            }
                                        }
                                    }
                                    else if (CheckedItem.Equals(avatar.Backpack[5]))
                                    {
                                        if (avatar.Backpack[5].ItType == ItemType.Others && avatar.Backpack[5].EfType == EffectType.Healing && avatar.CurrentHp != avatar.MaxHp)
                                        {
                                            avatar.CurrentHp += avatar.Backpack[5].ItemAttributeValue;
                                            avatar.Backpack[5] = new Item(avatar.Backpack[5].X, avatar.Backpack[5].Y, 255, ItemType.Others);
                                        }
                                        else
                                        {
                                            if (avatar.Equipment[0].Id == 255 && avatar.Backpack[5].ItType == ItemType.Weapon)
                                            {
                                                avatar.Attack += avatar.Backpack[5].ItemAttributeValue;
                                                avatar.Equipment[0] = new Item(avatar.Equipment[0].X, avatar.Equipment[0].Y, avatar.Backpack[5].Id, avatar.Backpack[5].ItType);
                                                avatar.Backpack[5] = new Item(avatar.Backpack[5].X, avatar.Backpack[5].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[1].Id == 255 && avatar.Backpack[5].ItType == ItemType.Weapon)
                                            {
                                                avatar.Attack += avatar.Backpack[5].ItemAttributeValue;
                                                avatar.Equipment[1] = new Item(avatar.Equipment[1].X, avatar.Equipment[1].Y, avatar.Backpack[5].Id, avatar.Backpack[5].ItType);
                                                avatar.Backpack[5] = new Item(avatar.Backpack[5].X, avatar.Backpack[5].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[2].Id == 255 && avatar.Backpack[5].ItType == ItemType.Armor)
                                            {
                                                avatar.Defense += avatar.Backpack[5].ItemAttributeValue;
                                                avatar.Equipment[2] = new Item(avatar.Equipment[2].X, avatar.Equipment[2].Y, avatar.Backpack[5].Id, avatar.Backpack[5].ItType);
                                                avatar.Backpack[5] = new Item(avatar.Backpack[5].X, avatar.Backpack[5].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[3].Id == 255 && avatar.Backpack[5].ItType == ItemType.Armor)
                                            {
                                                avatar.Defense += avatar.Backpack[5].ItemAttributeValue;
                                                avatar.Equipment[3] = new Item(avatar.Equipment[3].X, avatar.Equipment[3].Y, avatar.Backpack[5].Id, avatar.Backpack[5].ItType);
                                                avatar.Backpack[5] = new Item(avatar.Backpack[5].X, avatar.Backpack[5].Y, 255, ItemType.Others);
                                            }
                                        }
                                    }
                                    else if (CheckedItem.Equals(avatar.Backpack[6]))
                                    {
                                        if (avatar.Backpack[6].ItType == ItemType.Others && avatar.Backpack[6].EfType == EffectType.Healing && avatar.CurrentHp != avatar.MaxHp)
                                        {
                                            avatar.CurrentHp += avatar.Backpack[6].ItemAttributeValue;
                                            avatar.Backpack[6] = new Item(avatar.Backpack[6].X, avatar.Backpack[6].Y, 255, ItemType.Others);
                                        }
                                        else
                                        {
                                            if (avatar.Equipment[0].Id == 255 && avatar.Backpack[6].ItType == ItemType.Weapon)
                                            {
                                                avatar.Attack += avatar.Backpack[6].ItemAttributeValue;
                                                avatar.Equipment[0] = new Item(avatar.Equipment[0].X, avatar.Equipment[0].Y, avatar.Backpack[6].Id, avatar.Backpack[6].ItType);
                                                avatar.Backpack[6] = new Item(avatar.Backpack[6].X, avatar.Backpack[6].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[1].Id == 255 && avatar.Backpack[6].ItType == ItemType.Weapon)
                                            {
                                                avatar.Attack += avatar.Backpack[6].ItemAttributeValue;
                                                avatar.Equipment[1] = new Item(avatar.Equipment[1].X, avatar.Equipment[1].Y, avatar.Backpack[6].Id, avatar.Backpack[6].ItType);
                                                avatar.Backpack[6] = new Item(avatar.Backpack[6].X, avatar.Backpack[6].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[2].Id == 255 && avatar.Backpack[6].ItType == ItemType.Armor)
                                            {
                                                avatar.Defense += avatar.Backpack[6].ItemAttributeValue;
                                                avatar.Equipment[2] = new Item(avatar.Equipment[2].X, avatar.Equipment[2].Y, avatar.Backpack[6].Id, avatar.Backpack[6].ItType);
                                                avatar.Backpack[6] = new Item(avatar.Backpack[6].X, avatar.Backpack[6].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[3].Id == 255 && avatar.Backpack[6].ItType == ItemType.Armor)
                                            {
                                                avatar.Defense += avatar.Backpack[6].ItemAttributeValue;
                                                avatar.Equipment[3] = new Item(avatar.Equipment[3].X, avatar.Equipment[3].Y, avatar.Backpack[6].Id, avatar.Backpack[6].ItType);
                                                avatar.Backpack[6] = new Item(avatar.Backpack[6].X, avatar.Backpack[6].Y, 255, ItemType.Others);
                                            }
                                        }
                                    }
                                    else if (CheckedItem.Equals(avatar.Backpack[7]))
                                    {
                                        if (avatar.Backpack[7].ItType == ItemType.Others && avatar.Backpack[7].EfType == EffectType.Healing && avatar.CurrentHp != avatar.MaxHp)
                                        {
                                            avatar.CurrentHp += avatar.Backpack[7].ItemAttributeValue;
                                            avatar.Backpack[7] = new Item(avatar.Backpack[7].X, avatar.Backpack[7].Y, 255, ItemType.Others);
                                        }
                                        else
                                        {
                                            if (avatar.Equipment[0].Id == 255 && avatar.Backpack[7].ItType == ItemType.Weapon)
                                            {
                                                avatar.Attack += avatar.Backpack[7].ItemAttributeValue;
                                                avatar.Equipment[0] = new Item(avatar.Equipment[0].X, avatar.Equipment[0].Y, avatar.Backpack[7].Id, avatar.Backpack[7].ItType);
                                                avatar.Backpack[7] = new Item(avatar.Backpack[7].X, avatar.Backpack[7].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[1].Id == 255 && avatar.Backpack[7].ItType == ItemType.Weapon)
                                            {
                                                avatar.Attack += avatar.Backpack[7].ItemAttributeValue;
                                                avatar.Equipment[1] = new Item(avatar.Equipment[1].X, avatar.Equipment[1].Y, avatar.Backpack[7].Id, avatar.Backpack[7].ItType);
                                                avatar.Backpack[7] = new Item(avatar.Backpack[7].X, avatar.Backpack[7].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[2].Id == 255 && avatar.Backpack[7].ItType == ItemType.Armor)
                                            {
                                                avatar.Defense += avatar.Backpack[7].ItemAttributeValue;
                                                avatar.Equipment[2] = new Item(avatar.Equipment[2].X, avatar.Equipment[2].Y, avatar.Backpack[7].Id, avatar.Backpack[7].ItType);
                                                avatar.Backpack[7] = new Item(avatar.Backpack[7].X, avatar.Backpack[7].Y, 255, ItemType.Others);
                                            }
                                            else if (avatar.Equipment[3].Id == 255 && avatar.Backpack[7].ItType == ItemType.Armor)
                                            {
                                                avatar.Defense += avatar.Backpack[7].ItemAttributeValue;
                                                avatar.Equipment[3] = new Item(avatar.Equipment[3].X, avatar.Equipment[3].Y, avatar.Backpack[7].Id, avatar.Backpack[7].ItType);
                                                avatar.Backpack[7] = new Item(avatar.Backpack[7].X, avatar.Backpack[7].Y, 255, ItemType.Others);
                                            }
                                        }
                                    }
                                    PlayerStatusReload();
                                    ReloadRenderEq();
                                    RenderGameStateChange();
                                }
                                else if (ElementUsed == EquipmentElementUsed.Weared)
                                {
                                    ElementUsed = EquipmentElementUsed.Nothing;
                                    ///equipment 0 i 1 - bronie , 2 i 3 - zbroje
                                    if (CheckedItem.Equals(avatar.Equipment[0]))
                                    {
                                        for (int i = 0; i < 8; i++)
                                        {
                                            if (avatar.Backpack[i].Id == 255)
                                            {
                                                avatar.Backpack[i] = new Item(avatar.Backpack[i].X, avatar.Backpack[i].Y, avatar.Equipment[0].Id, ItemType.Weapon);
                                                avatar.Attack -= avatar.Equipment[0].ItemAttributeValue;
                                                avatar.Equipment[0] = new Item(avatar.Equipment[0].X, avatar.Equipment[0].Y, 255, ItemType.Others);
                                                break;
                                            }
                                        }
                                    }
                                    else if (CheckedItem.Equals(avatar.Equipment[1]))
                                    {
                                        for (int i = 0; i < 8; i++)
                                        {
                                            if (avatar.Backpack[i].Id == 255)
                                            {
                                                avatar.Backpack[i] = new Item(avatar.Backpack[i].X, avatar.Backpack[i].Y, avatar.Equipment[1].Id, ItemType.Weapon);
                                                avatar.Attack -= avatar.Equipment[1].ItemAttributeValue;
                                                avatar.Equipment[1] = new Item(avatar.Equipment[1].X, avatar.Equipment[1].Y, 255, ItemType.Others);
                                                break;
                                            }
                                        }
                                    }
                                    else if (CheckedItem.Equals(avatar.Equipment[2]))
                                    {
                                        for (int i = 0; i < 8; i++)
                                        {
                                            if (avatar.Backpack[i].Id == 255)
                                            {
                                                avatar.Backpack[i] = new Item(avatar.Backpack[i].X, avatar.Backpack[i].Y, avatar.Equipment[2].Id, ItemType.Armor);
                                                avatar.Defense -= avatar.Equipment[2].ItemAttributeValue;
                                                avatar.Equipment[2] = new Item(avatar.Equipment[2].X, avatar.Equipment[2].Y, 255, ItemType.Others);
                                                break;
                                            }
                                        }
                                    }
                                    else if (CheckedItem.Equals(avatar.Equipment[3]))
                                    {
                                        for (int i = 0; i < 8; i++)
                                        {
                                            if (avatar.Backpack[i].Id == 255)
                                            {
                                                avatar.Backpack[i] = new Item(avatar.Backpack[i].X, avatar.Backpack[i].Y, avatar.Equipment[3].Id, ItemType.Armor);
                                                avatar.Defense -= avatar.Equipment[3].ItemAttributeValue;
                                                avatar.Equipment[3] = new Item(avatar.Equipment[3].X, avatar.Equipment[3].Y, 255, ItemType.Others);
                                                break;
                                            }
                                        }
                                    }

                                    PlayerStatusReload();
                                    ReloadRenderEq();
                                    RenderGameStateChange();
                                }
                            }
                            else if (Mouse.X > 650 && Mouse.X < 710 && Mouse.Y > 200 && Mouse.Y < 220)
                            {
                                ///wyrzucanie przedmiotów
                                if (ElementUsed == EquipmentElementUsed.Backpack || ElementUsed == EquipmentElementUsed.Weared || ElementUsed == EquipmentElementUsed.Potion)
                                {
                                    ElementUsed = EquipmentElementUsed.Nothing;
                                    if (CheckingItemTexture != null) ObjectsToRender.Remove(CheckingItemTexture);
                                    if (DialogWindow != null) ObjectsToRender.Remove(DialogWindow);

                                    if (CheckedItem.Equals(avatar.Potions[0]))
                                    {
                                        avatar.Potions[0] = new Item(avatar.Potions[0].X, avatar.Potions[0].Y, 255, ItemType.Others);
                                    }
                                    else if (CheckedItem.Equals(avatar.Potions[1]))
                                    {
                                        avatar.Potions[1] = new Item(avatar.Potions[1].X, avatar.Potions[1].Y, 255, ItemType.Others);
                                    }
                                    else if (CheckedItem.Equals(avatar.Backpack[0]))
                                    {
                                        avatar.Backpack[0] = new Item(avatar.Backpack[0].X, avatar.Backpack[0].Y, 255, ItemType.Others);
                                    }
                                    else if (CheckedItem.Equals(avatar.Backpack[1]))
                                    {
                                        avatar.Backpack[1] = new Item(avatar.Backpack[1].X, avatar.Backpack[1].Y, 255, ItemType.Others);
                                    }
                                    else if (CheckedItem.Equals(avatar.Backpack[2]))
                                    {
                                        avatar.Backpack[2] = new Item(avatar.Backpack[2].X, avatar.Backpack[2].Y, 255, ItemType.Others);
                                    }
                                    else if (CheckedItem.Equals(avatar.Backpack[3]))
                                    {
                                        avatar.Backpack[3] = new Item(avatar.Backpack[3].X, avatar.Backpack[3].Y, 255, ItemType.Others);
                                    }
                                    else if (CheckedItem.Equals(avatar.Backpack[4]))
                                    {
                                        avatar.Backpack[4] = new Item(avatar.Backpack[4].X, avatar.Backpack[4].Y, 255, ItemType.Others);
                                    }
                                    else if (CheckedItem.Equals(avatar.Backpack[5]))
                                    {
                                        avatar.Backpack[5] = new Item(avatar.Backpack[5].X, avatar.Backpack[5].Y, 255, ItemType.Others);
                                    }
                                    else if (CheckedItem.Equals(avatar.Backpack[6]))
                                    {
                                        avatar.Backpack[6] = new Item(avatar.Backpack[6].X, avatar.Backpack[6].Y, 255, ItemType.Others);
                                    }
                                    else if (CheckedItem.Equals(avatar.Backpack[7]))
                                    {
                                        avatar.Backpack[7] = new Item(avatar.Backpack[7].X, avatar.Backpack[7].Y, 255, ItemType.Others);
                                    }
                                    else if (CheckedItem.Equals(avatar.Equipment[0]))
                                    {
                                        if (avatar.Equipment[0].ItType == ItemType.Armor)
                                        {
                                            avatar.Defense -= avatar.Equipment[0].ItemAttributeValue;
                                        }
                                        else if (avatar.Equipment[0].ItType == ItemType.Weapon)
                                        {
                                            avatar.Attack -= avatar.Equipment[0].ItemAttributeValue;
                                        }
                                        avatar.Equipment[0] = new Item(avatar.Equipment[0].X, avatar.Equipment[0].Y, 255, ItemType.Others);
                                    }
                                    else if (CheckedItem.Equals(avatar.Equipment[1]))
                                    {
                                        if (avatar.Equipment[1].ItType == ItemType.Armor)
                                        {
                                            avatar.Defense -= avatar.Equipment[1].ItemAttributeValue;
                                        }
                                        else if (avatar.Equipment[1].ItType == ItemType.Weapon)
                                        {
                                            avatar.Attack -= avatar.Equipment[1].ItemAttributeValue;
                                        }
                                        avatar.Equipment[1] = new Item(avatar.Equipment[1].X, avatar.Equipment[1].Y, 255, ItemType.Others);
                                    }
                                    else if (CheckedItem.Equals(avatar.Equipment[2]))
                                    {
                                        if (avatar.Equipment[2].ItType == ItemType.Armor)
                                        {
                                            avatar.Defense -= avatar.Equipment[2].ItemAttributeValue;
                                        }
                                        else if (avatar.Equipment[2].ItType == ItemType.Weapon)
                                        {
                                            avatar.Attack -= avatar.Equipment[2].ItemAttributeValue;
                                        }
                                        avatar.Equipment[2] = new Item(avatar.Equipment[2].X, avatar.Equipment[2].Y, 255, ItemType.Others);
                                    }
                                    else if (CheckedItem.Equals(avatar.Equipment[3]))
                                    {
                                        if (avatar.Equipment[3].ItType == ItemType.Armor)
                                        {
                                            avatar.Defense -= avatar.Equipment[3].ItemAttributeValue;
                                        }
                                        else if (avatar.Equipment[3].ItType == ItemType.Weapon)
                                        {
                                            avatar.Attack -= avatar.Equipment[3].ItemAttributeValue;
                                        }
                                        avatar.Equipment[3] = new Item(avatar.Equipment[3].X, avatar.Equipment[3].Y, 255, ItemType.Others);
                                    }
                                    PlayerStatusReload();
                                    ReloadRenderEq();


                                }
                            }
                            else if (Mouse.X > 650 && Mouse.X < 710 && Mouse.Y > 220 && Mouse.Y < 240)
                            {
                                ///anulowanie okna wyboru działania przedmiotu
                                if (ElementUsed == EquipmentElementUsed.Backpack || ElementUsed == EquipmentElementUsed.Weared || ElementUsed == EquipmentElementUsed.Potion)
                                {
                                    ElementUsed = EquipmentElementUsed.Nothing;
                                    if (CheckingItemTexture != null) ObjectsToRender.Remove(CheckingItemTexture);
                                    if (DialogWindow != null) ObjectsToRender.Remove(DialogWindow);
                                }
                            }

                            ///zaznaczanie przedmiotów
                            ///obsługa mikstur
                            if (Mouse.X > 743 && Mouse.X < 793 && Mouse.Y > 248 && Mouse.Y < 298)
                            {
                                CheckedItem = avatar.Potions[0];
                                if (CheckedItem != null)
                                {
                                    if (CheckedItem.Id != 255)
                                    {
                                        if (CheckingItemTexture != null) ObjectsToRender.Remove(CheckingItemTexture);
                                        if (DialogWindow != null) ObjectsToRender.Remove(DialogWindow);
                                        LoadingDialog = true;
                                        ElementUsed = EquipmentElementUsed.Potion;
                                        CheckingItemTexture = LoadInstantGuiTexture(avatar.Potions[0].X, avatar.Potions[0].Y, 15);
                                    }
                                }
                            }
                            else if (Mouse.X > 743 && Mouse.X < 793 && Mouse.Y > 300 && Mouse.Y < 350)
                            {
                                CheckedItem = avatar.Potions[1];
                                if (CheckedItem != null)
                                {
                                    if (CheckedItem.Id != 255)
                                    {
                                        if (CheckingItemTexture != null) ObjectsToRender.Remove(CheckingItemTexture);
                                        if (DialogWindow != null) ObjectsToRender.Remove(DialogWindow);
                                        LoadingDialog = true;
                                        ElementUsed = EquipmentElementUsed.Potion;
                                        CheckingItemTexture = LoadInstantGuiTexture(avatar.Potions[1].X, avatar.Potions[1].Y, 15);
                                    }
                                }
                            }
                            ///obsługa plecaka
                            else if (Mouse.X > 505 && Mouse.X < 555 && Mouse.Y > 248 && Mouse.Y < 298)
                            {
                                CheckedItem = avatar.Backpack[0];
                                if (CheckedItem != null)
                                {
                                    if (CheckedItem.Id != 255)
                                    {
                                        if (CheckingItemTexture != null) ObjectsToRender.Remove(CheckingItemTexture);
                                        if (DialogWindow != null) ObjectsToRender.Remove(DialogWindow);
                                        LoadingDialog = true;
                                        ElementUsed = EquipmentElementUsed.Backpack;
                                        CheckingItemTexture = LoadInstantGuiTexture(avatar.Backpack[0].X, avatar.Backpack[0].Y, 15);
                                    }
                                }
                            }
                            else if (Mouse.X > 554 && Mouse.X < 604 && Mouse.Y > 248 && Mouse.Y < 298)
                            {
                                CheckedItem = avatar.Backpack[1];
                                if (CheckedItem != null)
                                {
                                    if (CheckedItem.Id != 255)
                                    {
                                        if (CheckingItemTexture != null) ObjectsToRender.Remove(CheckingItemTexture);
                                        if (DialogWindow != null) ObjectsToRender.Remove(DialogWindow);
                                        LoadingDialog = true;
                                        ElementUsed = EquipmentElementUsed.Backpack;
                                        CheckingItemTexture = LoadInstantGuiTexture(avatar.Backpack[1].X, avatar.Backpack[1].Y, 15);
                                    }
                                }
                            }
                            else if (Mouse.X > 604 && Mouse.X < 654 && Mouse.Y > 248 && Mouse.Y < 298)
                            {
                                CheckedItem = avatar.Backpack[2];
                                if (CheckedItem != null)
                                {
                                    if (CheckedItem.Id != 255)
                                    {
                                        if (CheckingItemTexture != null) ObjectsToRender.Remove(CheckingItemTexture);
                                        if (DialogWindow != null) ObjectsToRender.Remove(DialogWindow);
                                        LoadingDialog = true;
                                        ElementUsed = EquipmentElementUsed.Backpack;
                                        CheckingItemTexture = LoadInstantGuiTexture(avatar.Backpack[2].X, avatar.Backpack[2].Y, 15);
                                    }
                                }
                            }
                            else if (Mouse.X > 654 && Mouse.X < 704 && Mouse.Y > 248 && Mouse.Y < 298)
                            {
                                CheckedItem = avatar.Backpack[3];
                                if (CheckedItem != null)
                                {
                                    if (CheckedItem.Id != 255)
                                    {
                                        if (CheckingItemTexture != null) ObjectsToRender.Remove(CheckingItemTexture);
                                        if (DialogWindow != null) ObjectsToRender.Remove(DialogWindow);
                                        LoadingDialog = true;
                                        ElementUsed = EquipmentElementUsed.Backpack;
                                        CheckingItemTexture = LoadInstantGuiTexture(avatar.Backpack[3].X, avatar.Backpack[3].Y, 15);
                                    }
                                }
                            }
                            else if (Mouse.X > 505 && Mouse.X < 555 && Mouse.Y > 300 && Mouse.Y < 350)
                            {
                                CheckedItem = avatar.Backpack[4];
                                if (CheckedItem != null)
                                {
                                    if (CheckedItem.Id != 255)
                                    {
                                        if (CheckingItemTexture != null) ObjectsToRender.Remove(CheckingItemTexture);
                                        if (DialogWindow != null) ObjectsToRender.Remove(DialogWindow);
                                        LoadingDialog = true;
                                        ElementUsed = EquipmentElementUsed.Backpack;
                                        CheckingItemTexture = LoadInstantGuiTexture(avatar.Backpack[4].X, avatar.Backpack[4].Y, 15);
                                    }
                                }
                            }
                            else if (Mouse.X > 554 && Mouse.X < 604 && Mouse.Y > 300 && Mouse.Y < 350)
                            {
                                CheckedItem = avatar.Backpack[5];
                                if (CheckedItem != null)
                                {
                                    if (CheckedItem.Id != 255)
                                    {
                                        if (CheckingItemTexture != null) ObjectsToRender.Remove(CheckingItemTexture);
                                        if (DialogWindow != null) ObjectsToRender.Remove(DialogWindow);
                                        LoadingDialog = true;
                                        ElementUsed = EquipmentElementUsed.Backpack;
                                        CheckingItemTexture = LoadInstantGuiTexture(avatar.Backpack[5].X, avatar.Backpack[5].Y, 15);
                                    }
                                }
                            }
                            else if (Mouse.X > 604 && Mouse.X < 654 && Mouse.Y > 300 && Mouse.Y < 350)
                            {
                                CheckedItem = avatar.Backpack[6];
                                if (CheckedItem != null)
                                {
                                    if (CheckedItem.Id != 255)
                                    {
                                        if (CheckingItemTexture != null) ObjectsToRender.Remove(CheckingItemTexture);
                                        if (DialogWindow != null) ObjectsToRender.Remove(DialogWindow);
                                        LoadingDialog = true;
                                        ElementUsed = EquipmentElementUsed.Backpack;
                                        CheckingItemTexture = LoadInstantGuiTexture(avatar.Backpack[6].X, avatar.Backpack[6].Y, 15);
                                    }
                                }
                            }
                            else if (Mouse.X > 654 && Mouse.X < 704 && Mouse.Y > 300 && Mouse.Y < 350)
                            {
                                CheckedItem = avatar.Backpack[7];
                                if (CheckedItem != null)
                                {
                                    if (CheckedItem.Id != 255)
                                    {
                                        if (CheckingItemTexture != null) ObjectsToRender.Remove(CheckingItemTexture);
                                        if (DialogWindow != null) ObjectsToRender.Remove(DialogWindow);
                                        LoadingDialog = true;
                                        ElementUsed = EquipmentElementUsed.Backpack;
                                        CheckingItemTexture = LoadInstantGuiTexture(avatar.Backpack[7].X, avatar.Backpack[7].Y, 15);
                                    }
                                }
                            }
                            ///obsługa założonego ekwipunku
                            else if (Mouse.X > 694 && Mouse.X < 744 && Mouse.Y > 106 && Mouse.Y < 156)
                            {
                                CheckedItem = avatar.Equipment[0];
                                if (CheckedItem != null)
                                {
                                    if (CheckedItem.Id != 255)
                                    {
                                        if (CheckingItemTexture != null) ObjectsToRender.Remove(CheckingItemTexture);
                                        if (DialogWindow != null) ObjectsToRender.Remove(DialogWindow);
                                        LoadingDialog = true;
                                        ElementUsed = EquipmentElementUsed.Weared;
                                        CheckingItemTexture = LoadInstantGuiTexture(avatar.Equipment[0].X, avatar.Equipment[0].Y, 15);
                                    }
                                }
                            }
                            else if (Mouse.X > 745 && Mouse.X < 795 && Mouse.Y > 106 && Mouse.Y < 156)
                            {
                                CheckedItem = avatar.Equipment[1];
                                if (CheckedItem != null)
                                {
                                    if (CheckedItem.Id != 255)
                                    {
                                        if (CheckingItemTexture != null) ObjectsToRender.Remove(CheckingItemTexture);
                                        if (DialogWindow != null) ObjectsToRender.Remove(DialogWindow);
                                        LoadingDialog = true;
                                        ElementUsed = EquipmentElementUsed.Weared;
                                        CheckingItemTexture = LoadInstantGuiTexture(avatar.Equipment[1].X, avatar.Equipment[1].Y, 15);
                                    }
                                }
                            }
                            else if (Mouse.X > 745 && Mouse.X < 795 && Mouse.Y > 157 && Mouse.Y < 207)
                            {
                                CheckedItem = avatar.Equipment[2];
                                if (CheckedItem != null)
                                {
                                    if (CheckedItem.Id != 255)
                                    {
                                        if (CheckingItemTexture != null) ObjectsToRender.Remove(CheckingItemTexture);
                                        if (DialogWindow != null) ObjectsToRender.Remove(DialogWindow);
                                        LoadingDialog = true;
                                        ElementUsed = EquipmentElementUsed.Weared;
                                        CheckingItemTexture = LoadInstantGuiTexture(avatar.Equipment[2].X, avatar.Equipment[2].Y, 15);
                                    }
                                }
                            }
                            else if (Mouse.X > 745 && Mouse.X < 795 && Mouse.Y > 55 && Mouse.Y < 105)
                            {
                                CheckedItem = avatar.Equipment[3];
                                if (CheckedItem != null)
                                {
                                    if (CheckedItem.Id != 255)
                                    {
                                        if (CheckingItemTexture != null) ObjectsToRender.Remove(CheckingItemTexture);
                                        if (DialogWindow != null) ObjectsToRender.Remove(DialogWindow);
                                        LoadingDialog = true;
                                        ElementUsed = EquipmentElementUsed.Weared;
                                        CheckingItemTexture = LoadInstantGuiTexture(avatar.Equipment[3].X, avatar.Equipment[3].Y, 15);
                                    }
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
                        if (IsWin10())TmpText = LoadTextInput(540, 195, 22, this.PlayersName);
                        else { TmpText = LoadTextInput(540, 190, 22, this.PlayersName); }
                        LastTmpText = TmpText;
                        System.GC.Collect();
                        System.GC.WaitForPendingFinalizers();
                    }
                }
                else if (gameState == GameState.InGame)
                {
                      if (StatusDisplayText.Count > 0)
                        {
                            foreach (var item in StatusDisplayText.ToList())
                            {
                                TextToRender.Remove(item);
                            }      
                        }
                    if (Mouse.X > 0 && Mouse.X < 500 && Mouse.Y > 0 && Mouse.Y < 500)
                    {

                        var field = fields[(int)Mouse.X / 50, (int)Mouse.Y / 50];
                        StatusDisplayText.Add(LoadTextInput(510, 400, 18, field.Description));
                        if (fields[field.X / 50, field.Y / 50].TextBuffor != "")
                            StatusDisplayText.Add(LoadTextInput(510, 430, 18, fields[field.X / 50, field.Y / 50].TextBuffor));
                        System.GC.Collect();
                        System.GC.WaitForPendingFinalizers();
                    }
                    if (GamePlayed == true)
                    {///wyświetlanie opisów
                        ///obsługa mikstur
                        if (Mouse.X > 743 && Mouse.X < 793 && Mouse.Y > 248 && Mouse.Y < 298)
                        {
                            if (avatar.Potions[0].Id != 255)
                            {
                                StatusDisplayText.Add(LoadTextInput(510, 400, 18, avatar.Potions[0].Description));
                                StatusDisplayText.Add(LoadTextInput(510, 430, 18, "Leczenie: " + avatar.Potions[0].ItemAttributeValue));
                            }
                        }
                        else if (Mouse.X > 743 && Mouse.X < 793 && Mouse.Y > 300 && Mouse.Y < 350)
                        {
                            if (avatar.Potions[1].Id != 255)
                            {
                                StatusDisplayText.Add(LoadTextInput(510, 400, 18, avatar.Potions[1].Description));
                                StatusDisplayText.Add(LoadTextInput(510, 430, 18, "Leczenie: " + avatar.Potions[1].ItemAttributeValue));
                            }
                        }
                        ///obsługa plecaka
                        else if (Mouse.X > 505 && Mouse.X < 555 && Mouse.Y > 248 && Mouse.Y < 298)
                        {
                            if (avatar.Backpack[0].Id != 255)
                            {
                                StatusDisplayText.Add(LoadTextInput(510, 400, 18, avatar.Backpack[0].Description));
                                ItemDescriptionStatusHelper(avatar.Backpack[0]);
                            }
                        }
                        else if (Mouse.X > 554 && Mouse.X < 604 && Mouse.Y > 248 && Mouse.Y < 298)
                        {
                            if (avatar.Backpack[1].Id != 255)
                            {
                                StatusDisplayText.Add(LoadTextInput(510, 400, 18, avatar.Backpack[1].Description));
                                ItemDescriptionStatusHelper(avatar.Backpack[1]);
                            }
                        }
                        else if (Mouse.X > 604 && Mouse.X < 654 && Mouse.Y > 248 && Mouse.Y < 298)
                        {
                            if (avatar.Backpack[2].Id != 255)
                            {
                                StatusDisplayText.Add(LoadTextInput(510, 400, 18, avatar.Backpack[2].Description));
                                ItemDescriptionStatusHelper(avatar.Backpack[2]);
                            }
                        }
                        else if (Mouse.X > 654 && Mouse.X < 704 && Mouse.Y > 248 && Mouse.Y < 298)
                        {
                            if (avatar.Backpack[3].Id != 255)
                            {
                                StatusDisplayText.Add(LoadTextInput(510, 400, 18, avatar.Backpack[3].Description));
                                ItemDescriptionStatusHelper(avatar.Backpack[3]);
                            }
                        }
                        else if (Mouse.X > 505 && Mouse.X < 555 && Mouse.Y > 300 && Mouse.Y < 350)
                        {
                            if (avatar.Backpack[4].Id != 255)
                            {
                                StatusDisplayText.Add(LoadTextInput(510, 400, 18, avatar.Backpack[4].Description));
                                ItemDescriptionStatusHelper(avatar.Backpack[4]);
                            }
                        }
                        else if (Mouse.X > 554 && Mouse.X < 604 && Mouse.Y > 300 && Mouse.Y < 350)
                        {
                            if (avatar.Backpack[5].Id != 255)
                            {
                                StatusDisplayText.Add(LoadTextInput(510, 400, 18, avatar.Backpack[5].Description));
                                ItemDescriptionStatusHelper(avatar.Backpack[5]);
                            }
                        }
                        else if (Mouse.X > 604 && Mouse.X < 654 && Mouse.Y > 300 && Mouse.Y < 350)
                        {
                            if (avatar.Backpack[6].Id != 255)
                            {
                                StatusDisplayText.Add(LoadTextInput(510, 400, 18, avatar.Backpack[6].Description));
                                ItemDescriptionStatusHelper(avatar.Backpack[6]);
                            }
                        }
                        else if (Mouse.X > 654 && Mouse.X < 704 && Mouse.Y > 300 && Mouse.Y < 350)
                        {
                            if (avatar.Backpack[7].Id != 255)
                            {
                                StatusDisplayText.Add(LoadTextInput(510, 400, 18, avatar.Backpack[7].Description));
                                ItemDescriptionStatusHelper(avatar.Backpack[7]);
                            }
                        }
                        ///obsługa założonego ekwipunku
                        else if (Mouse.X > 694 && Mouse.X < 744 && Mouse.Y > 106 && Mouse.Y < 156)
                        {
                            if (avatar.Equipment[0].Id != 255)
                            {
                                StatusDisplayText.Add(LoadTextInput(510, 400, 18, avatar.Equipment[0].Description));
                                ItemDescriptionStatusHelper(avatar.Equipment[0]);
                            }
                        }
                        else if (Mouse.X > 745 && Mouse.X < 795 && Mouse.Y > 106 && Mouse.Y < 156)
                        {
                            if (avatar.Equipment[1].Id != 255)
                            {
                                StatusDisplayText.Add(LoadTextInput(510, 400, 18, avatar.Equipment[1].Description));
                                ItemDescriptionStatusHelper(avatar.Equipment[1]);
                            }
                        }
                        else if (Mouse.X > 745 && Mouse.X < 795 && Mouse.Y > 157 && Mouse.Y < 207)
                        {
                            if (avatar.Equipment[2].Id != 255)
                            {
                                StatusDisplayText.Add(LoadTextInput(510, 400, 18, avatar.Equipment[2].Description));
                                ItemDescriptionStatusHelper(avatar.Equipment[2]);
                            }
                        }
                        else if (Mouse.X > 745 && Mouse.X < 795 && Mouse.Y > 55 && Mouse.Y < 105)
                        {
                            if (avatar.Equipment[3].Id != 255)
                            {
                                StatusDisplayText.Add(LoadTextInput(510, 400, 18, avatar.Equipment[3].Description));
                                ItemDescriptionStatusHelper(avatar.Equipment[3]);
                            }
                        }
                    }
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
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
                    if (IsWin10()) TmpText = LoadTextInput(540, 195, 22, this.PlayersName);
                    else { TmpText = LoadTextInput(540, 190, 22, this.PlayersName); }
                    LastTmpText = TmpText;
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                }
            }
        }

        private void ItemDescriptionStatusHelper(Item item)
        {
            if (item.ItType == ItemType.Armor)
            {
                StatusDisplayText.Add(LoadTextInput(510, 430, 18, "Obrona: " + item.ItemAttributeValue));
            }
            else if (item.ItType == ItemType.Weapon)
            {
                StatusDisplayText.Add(LoadTextInput(510, 430, 18, "Atak: " + item.ItemAttributeValue));
            }
            else if (item.ItType == ItemType.Others)
            {
                if (item.EfType == EffectType.Healing) StatusDisplayText.Add(LoadTextInput(510, 430, 18, "Leczenie: " + item.ItemAttributeValue));
            }    
        }

        private void IncreaseHealingEfficiety()
        {
            ///zwiększa z levelem postaci wydajność leczenia
            if (avatar.Level != Avatars_Comparison_Level)
            {
                Avatars_Comparison_Level = avatar.Level;
                XMLParser parser = new XMLParser("Items.xml");
                for (int i = 0; i < GeneratedItems.Count; i++)
                {
                    if (GeneratedItems[i].ItType == ItemType.Others && GeneratedItems[i].EfType == EffectType.Healing)
                    {
                        var tmp = Int32.Parse(parser.getElementByAttribute("id", "" + GeneratedItems[i].Id, "effectStrength"));
                        GeneratedItems[i].ItemAttributeValue = (int)(tmp * (1 + 0.05 * (avatar.Level - 1)));
                    }
                }

                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (fields[i, j].Id == 4 && fields[i, j].Explored == true)
                        {
                            Item tmpItem = GeneratedItems.Where(x => x.X == fields[i, j].X && x.Y == fields[i, j].Y).FirstOrDefault();
                            if (tmpItem != null)
                            {
                                if (tmpItem.ItType == ItemType.Others && tmpItem.EfType == EffectType.Healing) fields[i, j].TextBuffor = "Leczenie: " + tmpItem.ItemAttributeValue;
                            }
                        }
                    }
                }
                for (int i = 0; i < avatar.Backpack.Length; i++)
                {
                    if (avatar.Backpack[i].Id != 255)
                    {
                        if (avatar.Backpack[i].ItType == ItemType.Others && avatar.Backpack[i].EfType == EffectType.Healing)
                        {
                            var tmp = Int32.Parse(parser.getElementByAttribute("id", "" + avatar.Backpack[i].Id, "effectStrength"));
                            avatar.Backpack[i].ItemAttributeValue = (int)(tmp * (1 + 0.05 * (avatar.Level - 1)));
                        }
                    }
                }
                for (int i = 0; i < avatar.Potions.Length; i++)
                {
                    if (avatar.Potions[i].Id != 255)
                    {
                        if (avatar.Potions[i].ItType == ItemType.Others && avatar.Potions[i].EfType == EffectType.Healing)
                        {
                            var tmp = Int32.Parse(parser.getElementByAttribute("id", "" + avatar.Potions[i].Id, "effectStrength"));
                            avatar.Potions[i].ItemAttributeValue = (int)(tmp * (1 + 0.05 * (avatar.Level - 1)));
                        }
                    }
                }
            }
        }







        private bool isMonsterNearby(int x, int y)
        {
            int CPX = x/50; ///przypisanie x
            int CPY = y/50; ///przypisanie y

            if (CPX >= 0 && CPX <= 9 && CPY >= 0 && CPY <= 9)
            {
                if((CPX < 9 && (fields[CPX + 1,CPY].Id == 5) && (fields[CPX  + 1,CPY].Explored == true)) || ///potwór po prawej
                   (CPX > 0 && (fields[CPX - 1,CPY].Id == 5) && (fields[CPX - 1,CPY].Explored == true)) || ///potwór po lewej
                   (CPY < 9 && (fields[CPX,CPY + 1].Id == 5) && (fields[CPX,CPY + 1].Explored == true)) || ///potwór z dołu
                   (CPY > 0 && (fields[CPX,CPY - 1].Id == 5) && (fields[CPX,CPY - 1].Explored == true)) || ///potwór z góry   
                   (CPY > 0 && CPX > 0 && (fields[CPX - 1,CPY - 1].Id == 5) && (fields[CPX - 1,CPY - 1].Explored == true)) || ///potwór z lewej góry 
                   (CPY > 0 && CPX < 9 && (fields[CPX + 1,CPY - 1].Id == 5) && (fields[CPX + 1,CPY - 1].Explored == true)) || ///potwór z prawej góry  
                   (CPY < 9 && CPX > 0 && (fields[CPX - 1,CPY + 1].Id == 5) && (fields[CPX - 1,CPY + 1].Explored == true)) || ///potwór z lewego dołu
                   (CPY < 9 && CPX < 9 && (fields[CPX + 1,CPY + 1].Id == 5) && (fields[CPX + 1,CPY + 1].Explored == true))) ///potwór z prawego dołu
                {
                    return true;
                }
                else return false;
            }
            else return false;
        }

        private bool isExploredNearby(int x, int y)
        {
            int CPX = x / 50; ///przypisanie x
            int CPY = y / 50; ///przypisanie y

            if (CPX >= 0 && CPX <= 9 && CPY >= 0 && CPY <= 9)
            {
                if ((CPX < 9 && (fields[CPX + 1, CPY].Explored == true)) || ///odkryte pole po prawej
                   (CPX > 0 && (fields[CPX - 1, CPY].Explored == true)) || ///odkryte pole po lewej
                   (CPY < 9 && (fields[CPX, CPY + 1].Explored == true)) || ///odkryte pole z dołu
                   (CPY > 0 && (fields[CPX, CPY - 1].Explored == true))) ///odkryte pole z góry   
                {
                    return true;
                }
                else return false;
            }
            else return false;
        }
        
        private void LoadToRenderInterfaceObjects()
        {
            ///ładowanie zawartości bufora obiektów interfejsu
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


        
        private void ClearRenderer(BufforClear Option)
        {
            /// czyszczenie renderera lub jego elementów
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
                for (int i = 0; i < EquipmentObjects.Length; i++)
                {
                   if (EquipmentObjects[i] != null) ObjectsToRender.Remove(EquipmentObjects[i]);
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
            ///reakcje na zmiany stanów i inputy
            if (gameState == GameState.InMenu)
            {
                if (Loading == true)
                {
                    Loading = false;
                    LoadBackground(0);
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
                    LoadBackground(0);
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
                    LoadBackground(0);
                    LoadGuiTexture(500, 0, 10);
                    LoadText(515, 50, 25, "Wczytaj grę:");  
                    RenderReload();
                }
            }
            else if (gameState == GameState.InHighScore)
            {
                if (Loading == true)
                {
                    this.GamePlayed = false;
                    Loading = false;
                    LoadBackground(0);
                    LoadGuiTexture(500, 0, 07);
                    LoadText(515, 30, 25, "Najlepsi gracze:");  
                    ///ładowanie wyników
                    DisplayScores();
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
                    if (CheckingItemTexture != null)
                    {
                        ObjectsToRender.Remove(CheckingItemTexture);
                        ObjectsToRender.Add(CheckingItemTexture);
                    }
                    if (DialogWindow != null)
                    {
                        ObjectsToRender.Remove(DialogWindow);
                        ObjectsToRender.Add(DialogWindow);
                    }
                }
                else if (ElementUsed == EquipmentElementUsed.Potion && LoadingDialog == true)
                {
                    LoadingDialog = false;
                    DialogWindow = LoadInstantGuiTexture(650, 180, 13);
                    
                }
                else if (ElementUsed == EquipmentElementUsed.Backpack && LoadingDialog == true)
                {
                    LoadingDialog = false;
                    DialogWindow = LoadInstantGuiTexture(650, 180, 14);
                }
                else if (ElementUsed == EquipmentElementUsed.Weared && LoadingDialog == true)
                {
                    LoadingDialog = false;
                    DialogWindow = LoadInstantGuiTexture(650, 180, 12);
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
            else if (gameState == GameState.EndGame)
            {
                ///ładowanie elementów stanu końca gry
                if (Loading == true)
                {
                    Loading = false;
                    LoadBackground(0);
                    LoadGuiTexture(500, 0, 07);
                    if (WonTheGame == false)
                    {
                        LoadText(140, 100, 52, "Porażka!");
                    }
                    else
                    {
                        LoadText(110, 30, 52, "Zwycięstwo");
                        LoadText(60, 120, 32, "Gratulacje, przeszedłeś grę!");
                        LoadText(5, 260, 24, "Serdecznie dziękujęmy Ci za przejście naszej gry.");
                        LoadText(5, 300, 22, "Silnik oraz mechanika gry: Krzysztof Winiarski");
                        LoadText(5, 330, 22, "Oprawa graficzna gry: Magdalena Pietraszewska");
                        LoadText(5, 360, 22, "Algorytmy oraz project management: Piotr Trautman");
                        LoadText(5, 390, 22, "Dokumentacja: Paweł Dudzik");
                    }
                    
                    if (50 - (this.avatar.Experience.ToString().ToList().Count * 10) >= 0)
                    {
                        LoadText(50 - (this.avatar.Experience.ToString().ToList().Count * 10), 170, 44, "Twój wynik wynosi: " + avatar.Experience);
                    }
                    else
                    {
                        LoadText(50 - (this.avatar.Experience.ToString().ToList().Count * 5), 170, 40, "Twój wynik wynosi: " + avatar.Experience);
                    }
                    LoadText(515, 30, 25, "Najlepsi gracze:");

                    DisplayScores();
                    RenderReload();
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

        private void DisplayScores()
        {
            ///ładowanie wyników
            if (!(Directory.Exists(@"scores"))) Directory.CreateDirectory(@"scores");

            List<Score> ScoreList = new List<Score>();
            if (File.Exists(@"scores\\gamescores"))
            {
                string[] lines = File.ReadAllLines(@"scores\\gamescores");
                foreach (var item in lines)
                {
                    Score sctmp = new Score();
                    sctmp.Exp = Int32.Parse(item.Split(' ').Last());
                    int index = item.IndexOf(sctmp.Exp.ToString());
                    if (index > -1)
                    {
                        sctmp.Name = item.Remove(index - 1);
                    }
                    sctmp.ThisGame = false;
                    ScoreList.Add(sctmp);
                }
            }

            if (GamePlayed == true)
            {
                Score thisScore = new Score();
                thisScore.Name = PlayersName;
                thisScore.Exp = avatar.Experience;
                thisScore.ThisGame = true;
                ScoreList.Add(thisScore);
            }


            StreamWriter filewriter = new StreamWriter(@"scores\\gamescores");
            foreach (var item in ScoreList)
            {
                filewriter.WriteLine(item.Name + " " + item.Exp);
            }
            filewriter.Close();

            var templist = ScoreList.OrderByDescending(s => s.Exp);
            int counter = 0;
            foreach (var item in templist)
            {
                if (counter == 18) break;
                if (item.ThisGame == true)
                {
                    LoadText(520, 72 + 20 * counter, 18, (counter + 1) + ". " + item.Name + " - " + item.Exp, Color.Yellow);
                }
                else
                {
                    LoadText(520, 72 + 20 * counter, 18, (counter + 1) + ". " + item.Name + " - " + item.Exp);
                }
                counter++;
            }
        }

        private void InitGame()
        {
            this.LoadingDialog = false;
            this.avatar = new Avatar(this.PlayersName);
            this.GamePlayed = true;
            this.EquipmentObjects = new InitializedObjectTexture[14];
            this.ElementUsed = EquipmentElementUsed.Nothing;
            Avatars_Comparison_Level = this.avatar.Level;
            this.Dungeon_Level = 1;
            this.fields = new Field[10,10];
            this.Item_Enchant = 1;
            this.WonTheGame = false;
            ///generowanie tła
            LoadBackground(1);
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
            ///generowanie wejścia
            Random tempRand = new Random();
            int tempinX = tempRand.Next(0, 10);
            int tempinY = tempRand.Next(0, 10);
            LoadFieldTexture(tempinX, tempinY, 2, true);
            Console.WriteLine("wygenerowano poczatek na:" + tempinX + " " + tempinY);
            ///generowanie wyjścia
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
            ///generowanie klucza
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
            ReloadGame();
        }

        private void InitNextLevel()
        {
            this.Dungeon_Level += 1;
            if (Dungeon_Level == 21)
            {
                WonTheGame = true;
                lastGameState = gameState;
                gameState = GameState.EndGame;
                Loading = true;
            }
            else
            {
                if (this.Dungeon_Level % 5 == 1) this.Item_Enchant += 1;
                avatar.HasAKey = false;
                this.GeneratedItems = new List<Item>();
                this.GeneratedMonsters = new List<Monster>();
                this.Texts = new List<Text>();
                this.fields = new Field[10, 10];
                ///generowanie tła
                if (Dungeon_Level > 0 && Dungeon_Level <= 5)
                {
                    LoadBackground(1);
                }
                else if (Dungeon_Level > 5 && Dungeon_Level <= 10)
                {
                    LoadBackground(2);
                }
                else if (Dungeon_Level > 10 && Dungeon_Level <= 15)
                {
                    LoadBackground(3);
                }
                else if (Dungeon_Level > 15 && Dungeon_Level <= 20)
                {
                    LoadBackground(3,Color.Indigo);
                }

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
                ///generowanie wejścia
                Random tempRand = new Random();
                int tempinX = tempRand.Next(0, 10);
                int tempinY = tempRand.Next(0, 10);
                LoadFieldTexture(tempinX, tempinY, 2, true);
                Console.WriteLine("wygenerowano poczatek na:" + tempinX + " " + tempinY);
                ///generowanie wyjścia
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
                ///generowanie klucza
                Boss_Key_X = tempRand.Next(0, 10);
                Boss_Key_Y = tempRand.Next(0, 10);
                while (true)
                {
                    if ((((tempinX - Boss_Key_X) > 2) || ((tempinY - Boss_Key_Y) > 2) || ((tempinX - Boss_Key_X) < -2) || ((tempinY - Boss_Key_Y) < -2)) &&
                        (((tempoutX - Boss_Key_X) > 2) || ((tempoutY - Boss_Key_Y) > 2) || ((tempoutX - Boss_Key_X) < -2) || ((tempoutY - Boss_Key_Y) < -2)))
                    {
                        if (Dungeon_Level % 5 == 0)
                        {
                            Console.WriteLine("Boss level");
                            LoadFieldTexture(Boss_Key_X, Boss_Key_Y, 5, false);
                        }
                        else
                        {
                            LoadFieldTexture(Boss_Key_X, Boss_Key_Y, 4, false);
                        }

                        Console.WriteLine("wygenerowano klucz/bossa na:" + Boss_Key_X + " " + Boss_Key_Y);
                        break;
                    }
                    Boss_Key_X = tempRand.Next(0, 10);
                    Boss_Key_Y = tempRand.Next(0, 10);
                }
                ReloadGame();
                RenderReload();
                ReloadRenderEq();
                if (Dungeon_Level % 5 == 0)
                {
                    ///tekst boss level
                    BossAlert = LoadTextInput(20, 140, 72, "BOSS LEVEL!", Color.Red);
                    timer = new Timer();
                    timer.Interval = 3000;
                    timer.Elapsed += new ElapsedEventHandler(this.RemoveBossText);
                    timer.Start(); 
                }
                if (ElementUsed != EquipmentElementUsed.Nothing)
                {
                    if (CheckingItemTexture != null)
                    {
                        ObjectsToRender.Remove(CheckingItemTexture);
                        ObjectsToRender.Add(CheckingItemTexture);
                    }
                    if (DialogWindow != null)
                    {
                        ObjectsToRender.Remove(DialogWindow);
                        ObjectsToRender.Add(DialogWindow);
                    }
                }
            }
        }

        private void RemoveBossText(object sender, EventArgs e)
        {
            TextToRender.Remove(BossAlert);
            timer.Stop();      
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
            LoadText(610, 160, 16, this.Dungeon_Level + "");
            LoadText(610, 184, 16, this.avatar.Level + "");
        }

        private void LoadText(int x, int y, int fontSize, String txt)
        {
            int tempY = y;
            if (IsWin10()) tempY += 10;
            Text tekst = new Text(x, tempY, fontSize, txt);
            InitializedObjectTexture tekstTexture = new InitializedObjectTexture(tekst.X, tekst.Y, tekst.SizeX, tekst.SizeY, tekst.GeneratedBMP, Color.White);
            TextObjects.Add(tekstTexture);
        }

        private void LoadText(int x, int y, int fontSize, String txt, Color color)
        {
            int tempY = y;
            if (IsWin10()) tempY += 10;
            Text tekst = new Text(x, tempY, fontSize, txt);
            InitializedObjectTexture tekstTexture = new InitializedObjectTexture(tekst.X, tekst.Y, tekst.SizeX, tekst.SizeY, tekst.GeneratedBMP, color);
            TextObjects.Add(tekstTexture);
        }

        private InitializedObjectTexture LoadTextInput(int x, int y, int fontSize, String txt, Color color)
        {
            int tempY = y;
            if (IsWin10()) tempY += 10;
            Text tekst = new Text(x, tempY, fontSize, txt);
            InitializedObjectTexture tekstTexture = new InitializedObjectTexture(tekst.X, tekst.Y, tekst.SizeX, tekst.SizeY, tekst.GeneratedBMP, color);
            TextToRender.Add(tekstTexture);
            return tekstTexture;
        }

        private InitializedObjectTexture LoadTextInput(int x, int y, int fontSize, String txt)
        {
            int tempY = y;
            if (IsWin10()) tempY += 10;
            Text tekst = new Text(x, tempY, fontSize, txt);
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

        private InitializedObjectTexture LoadItemEquipmentTexture(int x, int y, int id, ItemType type)
        {
            Item item = new Item(x, y, id, type);
            ReferenceItem = item;
            GeneratedItems.Add(item);
            InitializedObjectTexture ItemTexture = new InitializedObjectTexture(item.X, item.Y, item.SizeX, item.SizeY, item.Path, Color.White);
            GameObjects.Add(ItemTexture);
            return ItemTexture;
        }

        private InitializedObjectTexture LoadMonsterTexture(int x, int y, int id, bool boss)
        {
            Monster monster = new Monster(x * 50, y * 50, id, boss);
            if (monster.IsBoss == false)
            {
                monster.Attack = (int)(monster.Attack * Math.Pow(1.2, this.Dungeon_Level - 1));
                monster.Defense = (int)(monster.Defense * Math.Pow(1.2, this.Dungeon_Level - 1));
                monster.MaxHP = (int)(monster.MaxHP * Math.Pow(1.2, this.Dungeon_Level - 1));
                monster.CurrentHP = monster.MaxHP;
                monster.Experience = (int)(monster.Experience * Math.Pow(1.2, this.Dungeon_Level - 1));
            }
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

        private InitializedObjectTexture LoadInstantGuiTexture(int x, int y, int id)
        {
            Background background = new Background(x, y, id, BackgroundType.GUI);
            InitializedObjectTexture Menu = new InitializedObjectTexture(background.X, background.Y, background.SizeX, background.SizeY, background.Path, Color.White);
            ObjectsToRender.Add(Menu);
            return Menu;
        }

        private void LoadBackground(int id)
        {
            Background Background = new Background(0, 0, id, BackgroundType.Board);
            InitializedObjectTexture BackgroundTexture = new InitializedObjectTexture(Background.X, Background.Y, Background.SizeX, Background.SizeY, Background.Path, Color.White);
            GameObjects.Add(BackgroundTexture);
        }

        private void LoadBackground(int id, Color color)
        {
            Background Background = new Background(0, 0, id, BackgroundType.Board);
            InitializedObjectTexture BackgroundTexture = new InitializedObjectTexture(Background.X, Background.Y, Background.SizeX, Background.SizeY, Background.Path, color);
            GameObjects.Add(BackgroundTexture);
        }

        static bool IsWin10()
        {
            ///warunek, czy uruchamiamy naszą grę na windowsie 10
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            string productName = (string)reg.GetValue("ProductName");
            return productName.StartsWith("Windows 10");
        }

        private void PlayerStatusReload()
        {
            ///aktualizacja stanu statusu naszego gracza
            LoadText(600 - (this.PlayersName.ToList().Count * 2), 25, 14, this.PlayersName);
            LoadText(560, 45, 14, this.avatar.CurrentHp + "/" + this.avatar.MaxHp);
            LoadText(560, 68, 14, this.avatar.Defense + "");
            LoadText(560, 92, 14, this.avatar.Attack + "");
            LoadText(560, 115, 14, this.avatar.Experience + "");
            if (this.avatar.HasAKey) LoadText(560, 142, 14, "Zdobyty");
            else LoadText(560, 142, 14, "Brak");
            LoadText(610, 160, 16, this.Dungeon_Level + "");
            LoadText(610, 184, 16, this.avatar.Level + "");
            TextRenderReload();
        }

        private void RenderReload()
        {
            ///główne przeładowanie renderera
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
            ///renderowanie na stan zmiany stanu gry
            ClearRenderer(BufforClear.InterfaceObjects);
            LoadToRenderInterfaceObjects();
            ReloadRenderEq();
            LoadToRenderTextObjects();
            ClearInterfaceObjectBuffer();
            ClearTextObjectBuffer();
        }

        private void ReloadRenderEq()
        {
            ///renderowanie ekwipunku - reload
            if (gameState == GameState.InGame)
            {
                LoadEquipmentObjects();
                LoadToRenderEquipmentObjects();
            }
        }


        private void LoadToRenderEquipmentObjects()
        {
            for (int i = 0; i < TemporaryInitEqList.Count; i++)
            {
                if (TemporaryInitEqList[i] != null) ObjectsToRender.Remove(TemporaryInitEqList[i]);
            }
            TemporaryInitEqList = new List<InitializedObjectTexture>();
            for (int i = 0; i < EquipmentObjects.Length; i++)
            {
                if (EquipmentObjects[i] != null)
                {
                    ObjectsToRender.Add(EquipmentObjects[i]);
                    TemporaryInitEqList.Add(EquipmentObjects[i]);
                }
            }
        }
        /// 14 slotów w ekwipunku - 0 - 7 plecak, 8 - 9 mikstury, 10 - 13 - ekwipunek założony
        private void LoadEquipmentObjects()
        {
            for (int i = 0; i < avatar.Backpack.Length; i++)
            {
                if (avatar.Backpack[i].Id != 255) EquipmentObjects[i] = new InitializedObjectTexture(avatar.Backpack[i].X, avatar.Backpack[i].Y, avatar.Backpack[i].SizeX, avatar.Backpack[i].SizeY, avatar.Backpack[i].Path, Color.White);
                else EquipmentObjects[i] = null;
            }
            for (int i = 0; i < avatar.Potions.Length; i++)
            {
                if (avatar.Potions[i].Id != 255) EquipmentObjects[i+8] = new InitializedObjectTexture(avatar.Potions[i].X, avatar.Potions[i].Y, avatar.Potions[i].SizeX, avatar.Potions[i].SizeY, avatar.Potions[i].Path, Color.White);
                else EquipmentObjects[i+8] = null;
            }
            for (int i = 0; i < avatar.Equipment.Length; i++)
            {
                if (avatar.Equipment[i].Id != 255) EquipmentObjects[i + 10] = new InitializedObjectTexture(avatar.Equipment[i].X, avatar.Equipment[i].Y, avatar.Equipment[i].SizeX, avatar.Equipment[i].SizeY, avatar.Equipment[i].Path, Color.White);
                else EquipmentObjects[i+10] = null;
            }
        }

        private void TextRenderReload()
        {
            ClearTextRenderer();
            LoadToRenderTextObjects();
            ClearTextObjectBuffer();
        }

    }
}
