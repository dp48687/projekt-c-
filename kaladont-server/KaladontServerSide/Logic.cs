using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace KaladontServerSide
{
    /// <summary>
    /// Contains constants for player states
    /// </summary>
    public enum PlayerState
    {
        NOT_PLAYING, ON_MOVE, NOT_ON_MOVE, LOST, WON
    }

    /// <summary>
    /// Marks the language on dictionary
    /// </summary>
    public enum DictionaryLanguage
    {
        ENG, HRV
    }

    /// <summary>
    /// Runs the game
    /// </summary>
    public class Logic
    {
        Task gameTask;
        PlayerGroup playerGroup;
        public long startMillisTime = 3000;            // time for starting animation
        public bool wasStarted;

        /// <summary>
        /// Initializes the player group and starts the game
        /// </summary>
        /// <param name="playerGroup"></param>
        public Logic(PlayerGroup playerGroup)
        {
            this.playerGroup = playerGroup;
            gameTask = new Task(Run);
            gameTask.Start();
        }

        /// <summary>
        /// Job to run the game
        /// </summary>
        public void Run()
        {
            Player current = playerGroup._players.ElementAt(0);
            while (playerGroup._players.Count > 1)
            {
                if (playerGroup._isReady && !wasStarted)
                {
                    startMillisTime--;
                    if(startMillisTime<=0)
                    {
                        wasStarted = true;
                        playerGroup._currentString = playerGroup._dictionaryManager.generateFirstRandomString();
                    }
                }
                else
                {
                    startMillisTime = 3000;
                }
                if (playerGroup._isReady)
                {
                    current = playerGroup._players.ElementAt(0);
                    if (current.millisTime <= 0)
                    {
                        PlayerFailed(true, playerGroup._players.ElementAt(0));
                        if(playerGroup._dictionaryManager.playedWords.Count>1)
                        {
                            playerGroup._players.ElementAt(playerGroup._players.Count()-1).localPoints+=3;
                        }
                        if(playerGroup._players.Count()<2)
                        {
                            playerGroup._players.ElementAt(playerGroup._players.Count() - 1).localPoints += 5;
                            current = playerGroup._players.ElementAt(0);
                            break;
                        }
                        playerGroup._players.ForEach(player => player.millisTime = playerGroup._eachPlayerMillis);
                        current = playerGroup._players.ElementAt(0);
                        playerGroup._currentString = playerGroup._dictionaryManager.generateFirstRandomString();
                        continue;
                    }
                    else
                    {
                         current.millisTime--;
                         if (current.hasSetString)
                         {
                             if (playerGroup._dictionaryManager.isChosenStringCorrect(playerGroup._currentString.ToLower(), current.stringPlayed.ToLower()))
                             {
                                current.localPoints += 2;
                                current.hasSetString = false;
                                playerGroup._currentString = current.stringPlayed;
                                current.millisTime = playerGroup._eachPlayerMillis;
                                if (current.stringPlayed.ToLower().Equals("kaladont"))
                                {
                                    Player failed = playerGroup._players.ElementAt(playerGroup._players.Count()-1);
                                    playerGroup._players.Remove(failed);
                                    PlayerFailed(false, failed);
                                    wasStarted = false;
                                    playerGroup._currentString = "";
                                    current.localPoints += 3;
                                    if(playerGroup._players.Count()<2)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        current = playerGroup._players.ElementAt(1);
                                        Player player = playerGroup._players.ElementAt(0);
                                        playerGroup._players.Remove(player);
                                        playerGroup._players.Add(player);
                                    }
                                }
                             
                             }
                             else
                             {
                                PlayerFailed(false, current);
                                playerGroup._players.ForEach(player => player.millisTime = playerGroup._eachPlayerMillis);
                                wasStarted = false;
                             }
                         }
                    }
                }
            }
            playerGroup.winner = current;
        }

        /// <summary>
        /// Deletes the player who failed to chooose correct string / ran out time
        /// </summary>
        public void PlayerFailed(bool didRanOutOfTime, Player current)
        {
            current.playerState = PlayerState.LOST;
            if (didRanOutOfTime)
            {
                TimeFail();
            }
            else
            {
                IncorrectStringFail();
            }
        }

        /// <summary>
        /// Job that has to be done when player runs out of time
        /// This method is going to invoke client method to reproduce the animation 
        /// </summary>
        public void TimeFail()
        {

        }

        /// <summary>
        /// Job that has to be done when player commits an incopatible string
        /// This method is going to invoke client method to reproduce the animation 
        /// </summary>
        public void IncorrectStringFail()
        {

        }
    }

    /// <summary>
    /// Loads the dictionary
    /// </summary>
    public class DictionaryManager : IDictionaryManager
    {
        public List<String> words;
        public DictionaryLanguage dictionaryLanguage;
        public List<String> playedWords;

        public DictionaryManager(DictionaryLanguage dictionaryLanguage)
        {
            this.dictionaryLanguage = dictionaryLanguage;
            if (dictionaryLanguage.Equals(DictionaryLanguage.HRV))
            {
                words = loadDictionary("/resources/hrv.txt");
            }
            else
            {
                words = loadDictionary("/resources/eng.txt");
            }
        }

        /// <summary>
        /// Determines if the player has chosen the correct string
        /// </summary>
        /// <param name="current">Current word</param>
        /// <param name="input">Word that has been chosen by player</param>
        /// <returns>True if he has chosen the correct string, false otherwise</returns>
        public bool isChosenStringCorrect(String current, String input)
        {
            input = input.ToLower();
            current = current.ToLower();
            if(playedWords.Contains(input))
            {
                return false;
            }
            if(input.Count()<3)
            {
                return false;
            }
            String slog1 = "" + input.ElementAt(0) + input.ElementAt(1);
            String slog2 = "" + current.ElementAt(current.Count() - 2) + current.ElementAt(current.Count() - 1);
            if (!slog1.Equals(slog2))
            {
                return false;
            }
            if(readStringFromDictionary(input))
            {
                playedWords.Add(input);
                return true;
            }
            else
            {
                return false;
            }
        }

    

        /// <summary>
        /// Loads words from a .txt file
        /// </summary>
        /// <param name="path"></param>
        /// <returns>A list of loaded strings</returns>
        public List<String> loadDictionary(string path)
        {
            string line;
            List<String> words = new List<String>();
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                words.Add(line);
            }
            file.Close();
            return words;
        }

        /// <summary>
        /// Uses modified binary search algorithm to find the specified word.
        /// </summary>
        /// <param name="searched">Searched word</param>
        /// <returns>True if word is found, false otherwise.</returns>
        public bool readStringFromDictionary(string searched)
        {
            int dynamicIndex = words.Count() / 2;
            int tempPosition = words.Count() / 2;
            String temp = "";
            if (words.ElementAt(0).Equals(searched) || words.ElementAt(words.Count() - 1).Equals(searched))
            {
                return true;
            }
            while(true){
                if (dynamicIndex == 0)
                {
                    if (words.ElementAt(tempPosition).Equals(searched))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                int position = searched.CompareTo(words.ElementAt(tempPosition));
                if (position == 0)
                {
                    return true;
                }
                else if (position < 0)
                {
                    tempPosition -= dynamicIndex;
                }
                else
                {
                    tempPosition += dynamicIndex;
                }
                dynamicIndex /= 2;
            }
        }

        /// <summary>
        /// Generates the random string from a dictionary.
        /// This method is called every time the game starts or after the player fails.
        /// </summary>
        /// <returns>Randomly chosen string</returns>
        public string generateFirstRandomString()
        {
            Random rand = new Random();
            playedWords = new List<string>();
            playedWords.Add(words.ElementAt(rand.Next(100,words.Count()-100)));
            return playedWords.ElementAt(0);
        }
    }

    /// <summary>
    /// Represents one group of _players that intend to play game against each other
    /// </summary>
    public class PlayerGroup
    {
        public string _name { get; set; }
        public DictionaryManager _dictionaryManager { get; set; }
        public List<Player> _players = new List<Player>();
        public bool _isReady { get; set; }                            //ceka se pritisak na START button
        public Player _currentPlayer;
        public List<String> _list = new List<String>();
        public String _currentString { get; set; }
        public long _eachPlayerMillis { get; set; }
        public Player winner { get; set; }

        /// <summary>
        /// Initializes the group
        /// </summary>
        public PlayerGroup(string name, bool isCro)
        {
            _name = name;
            if(isCro)
            {
                _dictionaryManager = new DictionaryManager(DictionaryLanguage.HRV);
            }
            else
            {
                _dictionaryManager = new DictionaryManager(DictionaryLanguage.ENG);
            }
            _eachPlayerMillis = 30000;
            _isReady = true;
        }

        /// <summary>
        /// Adds player to the segment
        /// </summary>
        /// <param name="player"></param>
        /// <returns>true if it is added to segment, false otherwise</returns>
        public bool addPlayer(Player player)
        {
            if (_players.Contains(player))
            {
                return false;
            }
            else
            {
                _players.Add(player);
                return true;
            }
        }

    }

    /// <summary>
    /// Represents the player
    /// </summary>
    public class Player
    {
        public String name { get; set; }                //_players name
        public String id { get; set; }                  //_players id
        public String gamesPlayed { get; set; }         //played matches
        public String gamesWon { get; set; }            //won matches
        public String gamesLost { get; set; }           //lost matches
        public String globalPoints { get; set; }        //collected points by now
        public String localPoints { get; set; }         //points for this round
        public PlayerState playerState { get; set; }    //player state           
        public long millisTime;                         //remaining time in milliseconds
        public bool hasSetString { get; set; }          //this is set to true when player sends the string                  
        public String stringPlayed { get; set; }        //this is the string which player has played

        /// <summary>
        /// Initializes the values
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Id"></param>
        /// <param name="GamesPlayed"></param>
        /// <param name="GamesWon"></param>
        /// <param name="GamesLost"></param>
        public Player(String Name, String Id)
        {
            name = Name;
            id = Id;
            playerState = PlayerState.NOT_PLAYING;
            millisTime = 30000;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public String chooseString()
        {
            return null;
        }

        /// <summary>
        /// Overrides the default Equals() method which then determines if this
        /// object is equal to another, given as a parameter.
        /// </summary>
        /// <param name="obj">Object which is going to be tested</param>
        /// <returns>True if objects are equal, false otherwise</returns>
        public override bool Equals(object obj)
        {
            if (this == null && obj == null)
            {
                return true;
            }
            else if (this == null && obj != null)
            {
                return false;
            }
            else if (this != null && obj == null)
            {
                return false;
            }
            else
            {
                Player p = (Player)obj;
                if (this.name.Equals(p.name))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    /// <summary>
    /// Contains methods for managing the dictionaries
    /// </summary>
    public interface IDictionaryManager
    {
        /// <summary>
        /// Loads the directory to the ArraySegment
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <returns>List of loaded strings</returns>
        List<String> loadDictionary(String path);

        /// <summary>
        /// Searches directory with binary search
        /// </summary>
        /// <param name="searched">Searched keyword</param>
        bool readStringFromDictionary(String searched);

        /// <summary>
        /// Determines compatibility of the specifing string
        /// </summary>
        /// <returns>True if string is correct, false otherwise</returns>
        bool isChosenStringCorrect(String current, String input);

    }

}
