using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SchallyLilyTicketingSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true) {
                Console.WriteLine("Type 1 to view movie data");
                Console.WriteLine("Type 2 to add to movie data");
                Console.WriteLine("Type 0 to exit");
                int option = GetInt(true, 0, 2, "", "Number must be one of the aforementioned values");
                switch(option) {
                    case 1:
                        viewMovies();
                        break;
                    case 2:
                        addMovie();
                        break;
                    default:
                        Environment.Exit(1);
                        break;
                }
            }
        }

        static void viewMovies() {
            string[] files = new string[] {"data/movies.csv", "data/ratings.csv", "data/tags.csv", "data/links.csv"};
            /*foreach (var file in files) {
                if (!System.IO.File.Exists(file)) {
                    Console.WriteLine($"{file} does not exist!");
                    return;
                }
            }*/
            // Was originally going to do stuff with the other files too
            if (!System.IO.File.Exists(files[0])) {
                Console.WriteLine($"{files[0]} does not exist!");
                return;
            }

            string[] lines = File.ReadAllLines(files[0]);
            string[,] movieData = new string[lines.Length, 3];
            string pattern = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
            for (int i = 0; i < lines.Length; i++)
            {
                string[] parts = Regex.Split(lines[i], pattern);

                if (parts.Length == 3) {
                    for (int j = 0; j < parts.Length; j++)
                    {
                        parts[j] = parts[j].Trim('"');
                        movieData[i, j] = parts[j];
                    }
                } else {
                    Console.WriteLine($"Invalid data format in line {i+1}. Skipping...");
                }
            }

            for (int i = 0; i < movieData.GetLength(0); i++)
            {
                Console.WriteLine($"{movieData[i,1]}, {movieData[i,2]}");
            }
        }

        static void addMovie() {
            string[] files = new string[] {"data/movies.csv", "data/ratings.csv", "data/tags.csv", "data/links.csv"};
            if (!System.IO.File.Exists(files[0])) {
                Console.WriteLine($"{files[0]} does not exist!");
                return;
            }
            string title = GetString("Enter movie title:", "Title cannot be blank");

            if (movieExists(files[0], title))
            {
                Console.WriteLine("Movie already exists in the database.");
                return;
            }

            string genresInput = GetString("Enter the genre(s) of the movie (separated by commas):", "Movie must have at least one genre");
            string genres = string.Join("|", genresInput.Split(',').Select(g => g.Trim()));

            int id = GenerateNewID(files[0]);
            using (StreamWriter writer = File.AppendText(files[0]))
            {
                writer.WriteLine($"{id},{title},{genres}");
            }
            Console.WriteLine("Movie added successfully to the database.");
        }

        static bool movieExists(string file, string title)
        {
            string[] lines = File.ReadAllLines(file);
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (parts.Length >= 2 && parts[1] == title)
                {
                    return true; 
                }
            }
            return false; 
        }

        static int GenerateNewID(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            int maxMovieId = 0;
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');

                if (int.TryParse(parts[0], out int movieId))
                {
                    maxMovieId = Math.Max(maxMovieId, movieId);
                }
            }

            return maxMovieId + 1;
        }

        static int GetInt(bool restrictValues, int intMin, int intMax, string prompt, string errorMsg) {
        
            string? userString = "";
            int userInt = 0;
            bool repSuccess = false;
            do {
                Console.Write(prompt);
                userString = Console.ReadLine();
    
                if (Int32.TryParse(userString, out userInt)) {
                    if (restrictValues)
                    {
                        if (userInt >= intMin && userInt <= intMax) {
                            repSuccess = true;
                        }
                    }
                    else
                    {
                        repSuccess = true;
                    }
                }
    
                // Output error
                if (!repSuccess) {
                    Console.WriteLine(errorMsg);
                }
            } while(!repSuccess);
    
            return userInt;
    
        }

        public static string GetString(string prompt, string errorMsg) {

            string? userString = "";
            bool repSuccess = false;
            do
            {
                Console.Write(prompt);
                userString = Console.ReadLine();

                if (!String.IsNullOrEmpty(userString))
                {
                    repSuccess = true;
                }

                // Output error
                if (!repSuccess)
                {
                    Console.WriteLine(errorMsg);
                }
            } while (!repSuccess);

            return userString;

        }
    }
}