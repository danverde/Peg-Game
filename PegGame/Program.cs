using Newtonsoft.Json;
using PegGame;
using PegGame.models;

var Game = new Game();
List<Move> solution = Game.Solve(0, 4);

string sol = JsonConvert.SerializeObject(solution);
Console.WriteLine($"Solution: {sol}");
