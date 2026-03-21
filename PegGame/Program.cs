using Newtonsoft.Json;
using PegGame;
using PegGame.models;


var Game = new Game();
List<Move> solution = Game.Solve(3, 1);

string sol = JsonConvert.SerializeObject(solution);
Console.WriteLine(sol);



    
