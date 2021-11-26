using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NewspaperSellerModels;
using NewspaperSellerTesting;
using System.IO;

namespace NewspaperSellerSimulation
{
    public partial class Form1 : Form
    {
        readonly SimulationSystem SimulationSystem = new SimulationSystem();
        public Form1()
        {
            InitializeComponent();
            GetDataFromFile();
            // Start Simulation



            // End Simulation
            string testingResult = TestingManager.Test(SimulationSystem, Constants.FileNames.TestCase1);
            MessageBox.Show(testingResult);
        }
        //Read Data input from file
        public void GetDataFromFile()
        {
            
            String Path = "TestCases\\";
            // Choosing test case
            Path += "TestCase1.txt";
            string[] lines = File.ReadAllLines(Path);
            SimulationSystem.NumOfNewspapers = int.Parse(lines[1]);
            SimulationSystem.NumOfRecords = int.Parse(lines[4]);
            SimulationSystem.PurchasePrice = decimal.Parse(lines[7]);
            SimulationSystem.ScrapPrice = decimal.Parse(lines[10]);
            SimulationSystem.SellingPrice = decimal.Parse(lines[13]);
            SimulationSystem.DayTypeDistributions= AddDayTypeDis(lines,16);

            for (int i = 19; i < lines.Length; i++)
            {
                DemandDistribution demandDistribution = new DemandDistribution();
                string[] line = lines[i].Split(',', (char)StringSplitOptions.RemoveEmptyEntries);
                demandDistribution.Demand = int.Parse(line[0]);
                demandDistribution.DayTypeDistributions = AddDayTypeDis(lines, i);
                SimulationSystem.DemandDistributions.Add(demandDistribution);
                
            }
        }

        public List<DayTypeDistribution> AddDayTypeDis(string[] lines, int line_number) {
            
            List<DayTypeDistribution> dayTypeDistributionslist = new List<DayTypeDistribution>();
            string[] DayTypeProb = lines[line_number].Split(',', (char)StringSplitOptions.RemoveEmptyEntries);
            if (DayTypeProb.Length == 4) DayTypeProb = DayTypeProb.Where(w => w != lines[0]).ToArray();
            for (int i = 0; i < 3; i++)
            {

                DayTypeDistribution dayTypeDistribution = new DayTypeDistribution();
                dayTypeDistribution.DayType = (i == 0) ? Enums.DayType.Good : (i == 1) ? Enums.DayType.Fair : Enums.DayType.Poor;
                dayTypeDistribution.Probability = decimal.Parse(DayTypeProb[i]);
                dayTypeDistributionslist.Add(dayTypeDistribution);
            }

            return dayTypeDistributionslist;
        }
    }
    
    }
