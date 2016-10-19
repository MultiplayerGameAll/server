using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    public class ComunicationManager
    {
        private List<ThreadReaderMessage> readers = new List<ThreadReaderMessage>();

        private Dictionary<string, Game> games = new Dictionary<string, Game>();

        public void addReader(ThreadReaderMessage trm)
        {
            readers.Add(trm);
        }

        public void sendMessage(ThreadReaderMessage sender, Request request)
        {
            foreach (ThreadReaderMessage trm in readers)
            {
                if(trm != sender)
                {
                    trm.sendMessage(request);
                }
            }
        }

        public void createGame(ThreadReaderMessage sender, Request request)
        {
            Game game = new Game();
            Player player = new Player();
            player.nick = request.nick;
            game.owner = player;
            games.Add(request.nick, game);
        }

        public void listGames(ThreadReaderMessage trm)
        {
            List<Game> gamesList = games.Values.ToList();
            string json = JsonConvert.SerializeObject(gamesList) + "###";
            trm.sendMessage(json);
        }

        public void joinGame(ThreadReaderMessage sender, Request request)
        {
            Game game = games[request.body];

            Player player = new Player();
            player.nick = request.nick;
            game.player.Add(player);
        }
    }
}
