using BigTic.Models;
using System.Text.Json;
namespace TestProject
{
    public class UnitTest1
    {
        [Fact(DisplayName = "Distance Test")]
        public void Test1()
        {
            Game game = new(9);
            Assert.Equal(0, game.Distance(0, 0, 0, 0));
        }

        [Fact(DisplayName = "Collision (Grid axes) True Test")]

        public void Test2()
        {
            Game game = new(9);
            Assert.True(game.PointLine(10, 4, 0, 0, 100, 0, 5));
        }

        [Fact(DisplayName = "Collision (Grid axes) False Test")]

        public void Test3()
        {
            Game game = new(9);
            Assert.False(game.PointLine(10, 4, 0, 0, 0, 100, 5));
        }
        [Fact(DisplayName = "No axis was hit test")]
        public void Test4()
        {
            Game game = new(9);
            string exp = JsonSerializer.Serialize(game);
            game.CheckClick(0, 0, 1080, 114);
            Assert.Equal(exp, JsonSerializer.Serialize(game));
        }
        [Fact(DisplayName = "Vertical axis was hit test")]
        public void Test5()
        {
            Game control = new(9);
            control.verticalArcs[5][4].value = "1";
            control.currentMove++;
            control.player = 1;
            Game game = new(9);
            game.CheckClick(406, 369, 77.9, 738);
            Assert.Equal(JsonSerializer.Serialize(control), JsonSerializer.Serialize(game));
        }
        [Fact(DisplayName = "Horizontal axis was hit test")]
        public void Test6()
        {
            Game control = new(9);
            control.horizontalArcs[4][5].value = "1";
            control.currentMove++;
            control.player = 1;
            Game game = new(9);
            game.CheckClick(369, 406, 77.9, 738);
            Assert.Equal(JsonSerializer.Serialize(control), JsonSerializer.Serialize(game));
        }
        [Fact(DisplayName = "Grid was colored")]
        public void Test7() {
            Game control = new(9);
            control.horizontalArcs[4][4].value = "1";
            control.verticalArcs[4][4].value = "1";
            control.verticalArcs[5][4].value = "1";
            control.horizontalArcs[4][5].value = "1";
            control.field[4][4].value = "2";
            control.player = 1;
            control.currentMove = 4;
            control.score[1] = 3;
            Game game = new(9);
            game.CheckClick(369, 329, 77.9, 738);
            game.CheckClick(329, 369, 77.9, 738);
            game.CheckClick(406, 369, 77.9, 738);
            game.CheckClick(369, 406, 77.9, 738);
            Assert.Equal(JsonSerializer.Serialize(control), JsonSerializer.Serialize(game));
        }
    }
}