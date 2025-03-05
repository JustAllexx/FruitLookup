﻿using FruityLookup;
using FruityLookup.Entities;

namespace FruityLookup.Tests {
    public class InstanceTests {
        readonly FruityLookup fruityLookup = new();

        public static Fruit getAppleFruit() {
            //Fruity Vice apple taken from the website
            Nutrition appleNutrition = new Nutrition
            {
                calories = 52,
                fat = 0.4,
                sugar = 10.3,
                carbohydrates = 11.4,
                protein = 0.3,
            };

            Fruit apple = new Fruit
            {
                name = "Apple",
                id = 6,
                family = "Rosaceae",
                order = "Rosales",
                genus = "Malus",
                nutritions = appleNutrition
            };
            return apple;
        }

        public static Fruit getBananaFruit() {
            //Fruity Vice apple taken from the website
            Nutrition bananaNutrition = new Nutrition
            {
                calories = 96,
                fat = 0.2,
                sugar = 17.2,
                carbohydrates = 22,
                protein = 1,
            };

            Fruit banana = new Fruit
            {
                name = "Banana",
                id = 1,
                family = "Musaceae",
                order = "Zingiberales",
                genus = "Musa",
                nutritions = bananaNutrition
            };
            return banana;
        }

        private const string input1 = "apple";
        private const string input2 = "banana";
        [Fact]
        public async Task getFruitInformationAsync() {
            //Test that retrieves expected information and deserialises correctly
            Fruit factApple = getAppleFruit();
            Fruit? apple = await fruityLookup.getFruitInformationAsync(input1);
            Assert.NotNull(apple);
            Assert.Equal(factApple, apple);

            //Test we can handle more than one request
            Fruit factBanana = getBananaFruit();
            Fruit? banana = await fruityLookup.getFruitInformationAsync(input2);
            Assert.NotNull(banana);
            Assert.Equal(factBanana, banana);
        }

        private const string expectedOutput1 = """
            {"name":"Apple","id":6,"family":"Rosaceae","order":"Rosales","genus":"Malus","nutritions":{"calories":52,"fat":0.4,"sugar":10.3,"carbohydrates":11.4,"protein":0.3}}
            """;
        [Fact]
        public void ToJsonString() {
            Fruit apple = getAppleFruit();
            string directJson = apple.ToJsonString();
            string formatJson = apple.ToString("JS");
            Assert.Equal(directJson, formatJson);
            Assert.Equal(expectedOutput1, directJson);
        }
    }

    public class CLITests {

        private readonly string[] args1 = { "apple" };
        [Fact]
        public async Task SimpleOutput() {
            using(StringWriter sw = new()) {
                Console.SetOut(sw);
                await FruityLookupCLI.Main(args1);
                string output = sw.ToString().Trim();

                Assert.Contains("Apple", output); //Full Name
                Assert.Contains("6", output); //ID number
                Assert.Contains("Rosaceae", output); //Family (Biological Classification)
                Assert.Contains("10.3g", output); //Sugar
                Assert.Contains("11.4g", output); //Carbohydrates
            }   
        }
    }
}
