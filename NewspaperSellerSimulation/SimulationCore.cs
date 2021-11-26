using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewspaperSellerModels;


    namespace NewspaperSellerSimulation
    {
        public class SimulationCore
        {
            const int Min = 1;
            const int Max = 101;

            public SimulationCore(SimulationSystem simulationSystem)
            {
                this.simulationSystem = simulationSystem;
            }

            SimulationSystem simulationSystem;

            public void RunServer()
            {
                FillDayTypeDistribution();
                FillDemandDistribution();
                FillSimulationTable();

            }

            private void FillDayTypeDistribution()
            {
                List<DayTypeDistribution> DTD = simulationSystem.DayTypeDistributions;
                DTD[0].CummProbability = DTD[0].Probability;
                DTD[0].MinRange = 1;
                DTD[0].MaxRange = Convert.ToInt32(DTD[0].CummProbability * 100);

                for (int i = 1; i < DTD.Count; i++)
                {
                    DTD[i].CummProbability = DTD[i - 1].CummProbability + DTD[i].Probability;
                    DTD[i].MinRange = DTD[i - 1].MaxRange + 1;
                    DTD[i].MaxRange = Convert.ToInt32(DTD[i].CummProbability * 100);
                }

                simulationSystem.DayTypeDistributions = DTD;
            }

            private void FillDemandDistribution()
            {
                List<DemandDistribution> DD = simulationSystem.DemandDistributions;
                for (int i = 0; i < 3; i++)
                {
                    DD[0].DayTypeDistributions[i].CummProbability = DD[0].DayTypeDistributions[i].Probability;
                    DD[0].DayTypeDistributions[i].MinRange = 1;
                    DD[0].DayTypeDistributions[i].MaxRange = Convert.ToInt32(DD[0].DayTypeDistributions[i].CummProbability * 100);

                    for (int j = 1; j < DD.Count; j++)
                    {
                        DD[j].DayTypeDistributions[i].CummProbability =
                            DD[j].DayTypeDistributions[i].Probability + DD[j - 1].DayTypeDistributions[i].CummProbability;

                        DD[j].DayTypeDistributions[i].MinRange = DD[j - 1].DayTypeDistributions[i].MaxRange + 1;

                        DD[j].DayTypeDistributions[i].MaxRange = Convert.ToInt32(DD[j].DayTypeDistributions[i].CummProbability * 100);
                    }
                }

                simulationSystem.DemandDistributions = DD;
            }


            private void FillSimulationTable()
            {
                int day = 1;
                Random random = new Random();

                while (day <= simulationSystem.NumOfRecords)
                {
                    SimulationCase simulationCase = new SimulationCase();
                    simulationCase.DayNo = day++;
                    int randomNewsDay = random.Next(Min, Max);
                    simulationCase.RandomNewsDayType = randomNewsDay;

                    for (int i = 0; i < simulationSystem.DayTypeDistributions.Count; i++)
                    {
                        if (randomNewsDay >= simulationSystem.DayTypeDistributions[i].MinRange && randomNewsDay <= simulationSystem.DayTypeDistributions[i].MaxRange)
                        {
                            simulationCase.NewsDayType = simulationSystem.DayTypeDistributions[i].DayType;
                            break;
                        }
                    }

                    int randomDemand = random.Next(Min, Max);
                    simulationCase.RandomDemand = randomDemand;

                    for (int i = 0; i < simulationSystem.DemandDistributions.Count; i++)
                    {
                        int idx = ((int)simulationCase.NewsDayType);
                        int minRange = simulationSystem.DemandDistributions[i].DayTypeDistributions[idx].MinRange;
                        int maxRange = simulationSystem.DemandDistributions[i].DayTypeDistributions[idx].MaxRange;
                        if (randomDemand >= minRange && randomDemand <= maxRange)
                        {
                            simulationCase.Demand = simulationSystem.DemandDistributions[i].Demand;
                            break;
                        }
                    }

                    simulationCase.SalesProfit = Math.Min(simulationSystem.NumOfNewspapers, simulationCase.Demand) * simulationSystem.SellingPrice;
                    simulationCase.LostProfit = Math.Max(0, (simulationCase.Demand - simulationSystem.NumOfNewspapers)) * (simulationSystem.SellingPrice - simulationSystem.PurchasePrice);
                    simulationCase.ScrapProfit = Math.Max(0, (simulationSystem.NumOfNewspapers - simulationCase.Demand)) * simulationSystem.ScrapPrice;
                    simulationCase.DailyCost = simulationSystem.NumOfNewspapers * simulationSystem.PurchasePrice;

                    simulationCase.DailyNetProfit = simulationCase.SalesProfit - simulationCase.DailyCost - simulationCase.LostProfit + simulationCase.ScrapProfit;

                    // Performance Measures
                    simulationSystem.PerformanceMeasures.TotalCost += simulationCase.DailyCost;
                    simulationSystem.PerformanceMeasures.TotalLostProfit += simulationCase.LostProfit;
                    simulationSystem.PerformanceMeasures.TotalScrapProfit += simulationCase.ScrapProfit;
                    simulationSystem.PerformanceMeasures.TotalSalesProfit += simulationCase.SalesProfit;
                    simulationSystem.PerformanceMeasures.TotalNetProfit += simulationCase.DailyNetProfit;

                    if (simulationCase.LostProfit != 0) simulationSystem.PerformanceMeasures.DaysWithMoreDemand++;
                    if (simulationCase.ScrapProfit != 0) simulationSystem.PerformanceMeasures.DaysWithUnsoldPapers++;




                    simulationSystem.SimulationTable.Add(simulationCase);

                }
            }
        }
    }

