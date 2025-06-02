using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class Program
{
    //public static void Main()
    public static async Task Main(string[] args)
    {
        FootballGoalsCalculator calculator = new FootballGoalsCalculator();

             string team1Name = "Paris Saint-Germain";
            int year1 = 2013;
            int goals1 = await calculator.GetTotalGoals(team1Name, year1);
            Console.WriteLine($"Team {team1Name} scored {goals1} goals in {year1}");

            // Resultado 2
            string team2Name = "Chelsea";
            int year2 = 2014;
            int goals2 = await calculator.GetTotalGoals(team2Name, year2);
            Console.WriteLine($"Team {team2Name} scored {goals2} goals in {year2}");

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }


    public class FootballMatch
    {
        public string Competition { get; set; }
        public int Year { get; set; }
        public string Round { get; set; }
        public string Team1 { get; set; }
        public string Team2 { get; set; }
        public string Team1Goals { get; set; } 
        public string Team2Goals { get; set; } 
    }

    public class ApiResponse
    {
        public int Page { get; set; }
        public int Per_page { get; set; }
        public int Total { get; set; }
        public int Total_pages { get; set; }
        public List<FootballMatch> Data { get; set; }
    }

    public class FootballGoalsCalculator
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string BaseUrl = "https://jsonmock.hackerrank.com/api/football_matches";

        public async Task<int> GetTotalGoals(string teamName, int year)
        {
            int totalGoals = 0;
            int currentPage = 1;
            int totalPages = 2; 

            while (currentPage <= totalPages)
            {
                
                string urlTeam1 = $"{BaseUrl}?year={year}&team1={teamName}&page={currentPage}";
                ApiResponse responseTeam1 = await GetMatches(urlTeam1);
                if (responseTeam1 != null)
                {
                    foreach (var match in responseTeam1.Data)
                    {
                        if (match.Team1 == teamName && int.TryParse(match.Team1Goals, out int goals))
                        {
                            totalGoals += goals;
                        }
                    }
                    totalPages = responseTeam1.Total_pages;
                }
                else
                {
                    
                    Console.WriteLine($"Erro ao buscar partidas como Team1 na página {currentPage}.");
                    break; 
                }

                
                string urlTeam2 = $"{BaseUrl}?year={year}&team2={teamName}&page={currentPage}";
                ApiResponse responseTeam2 = await GetMatches(urlTeam2);
                if (responseTeam2 != null)
                {
                    foreach (var match in responseTeam2.Data)
                    {
                        if (match.Team2 == teamName && int.TryParse(match.Team2Goals, out int goals))
                        {
                            totalGoals += goals;
                        }
                    }
                   
                    totalPages = Math.Max(totalPages, responseTeam2.Total_pages);
                }
                else
                {
                    
                    Console.WriteLine($"Erro ao buscar partidas como Team2 na página {currentPage}.");
                    break; 
                }

                currentPage++;
            }

            return totalGoals;
        }

        private async Task<ApiResponse> GetMatches(string url)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); 

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Erro na requisição HTTP: {e.Message}");
                return null;
            }
            catch (JsonSerializationException e)
            {
                Console.WriteLine($"Erro na desserialização JSON: {e.Message}");
                return null;
            }
        }
    }

}