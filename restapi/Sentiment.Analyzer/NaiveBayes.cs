using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace Sentiment.Analyzer
{
    //in order to run the sentiment analyizer, make sure to have the SaveData file
    //then run the function loadLearningFull that is part of the NaiveBayes class
    //
    public class NaiveBayes
    {
        Dictionary<string, int> positiveSingle;
        Dictionary<string, int> positiveTuple;
        Dictionary<string, int> positiveTriple;

        Dictionary<string, int> negativeSingle;
        Dictionary<string, int> negativeTuple;
        Dictionary<string, int> negativeTriple;

        int negativeSentimentTotal;
        int positiveSentimentTotal;

        public NaiveBayes()
        {
            //go to the definition of this function to make sure the right paths are being called on the pSingle pDouble etc. files
            this.loadLearningFull();
        }

        public NaiveBayes(Dictionary<string, int> pSingle, Dictionary<string, int> pTuple, Dictionary<string, int> pTriple, Dictionary<string, int> nSingle, Dictionary<string, int> nTuple, Dictionary<string, int> nTriple, int numP, int numN)
        {
            positiveSingle = pSingle;
            positiveTuple = pTuple;
            positiveTriple = pTriple;

            negativeSingle = nSingle;
            negativeTuple = nTuple;
            negativeTriple = nTriple;

            negativeSentimentTotal = numN;
            positiveSentimentTotal = numP;
        }

        public Dictionary<string, int> loadDictionary(string fileLocation)
        {
            var lines = File.ReadLines(fileLocation);
            Dictionary<string, int> sentimentDictionary = new Dictionary<string, int>();

            foreach (var line in lines)
            {
                var term = line.Remove(line.LastIndexOf(' '));
                var value = line.Substring(line.LastIndexOf(' ') + 1);

                int valueInt;

                if (!int.TryParse(value, out valueInt))
                {
                    continue;
                }

                sentimentDictionary.Add(term, valueInt);
            }

            //Here I need to delete the contents of pslines otherwise it will take up too much memory


            return sentimentDictionary;
        }

        //returns true for a positive prediction, negative for negative prediction
        public bool makePrediction(string input)
        {
            Tokenizer tokenizeObj = new Tokenizer();
            List<string> tokenized = tokenizeObj.tokenize(input);
            List<int> posInstances = new List<int>(), negInstances = new List<int>();
            string tempTuple, tempTriple;

            //we check each token to see if it exists in the given dictionary, if it does, add the amount of instances to the vector
            //here we change 'i' depending on the start location of the text to be analyzed within the tokens
            for (int i = 3; i < tokenized.Count; i++)
            {
                //this means we have to make a tuple
                if (i < tokenized.Count - 1)
                {
                    //this makes tempTuple a way to search the dictionary for the given tuple
                    tempTuple = tokenized[i] + " " + tokenized[i + 1];
                    if (positiveTuple.ContainsKey(tempTuple))//  positiveTuple.find(tempTuple) != positiveTuple.end())
                    {
                        posInstances.Add(positiveTuple[tempTuple]);
                    }
                    else
                    {
                        posInstances.Add(1);
                    }

                    if (negativeTuple.ContainsKey(tempTuple))// negativeTuple.find(tempTuple) != negativeTuple.end())
                    {
                        negInstances.Add(negativeTuple[tempTuple]);
                    }
                    else
                    {
                        negInstances.Add(1);
                    }
                }
                //this means make a triple
                if (i < tokenized.Count - 2 && tokenized.Count != 1)
                {
                    //this makes tempTriple a way to search the dictionary for the given tuple
                    tempTriple = tokenized[i] + " " + tokenized[i + 1] + " " + tokenized[i + 2];
                    if (positiveTriple.ContainsKey(tempTriple))//  positiveTriple.find(tempTriple) != positiveTriple.end())
                    {
                        posInstances.Add(positiveTriple[tempTriple]);
                    }
                    else
                    {
                        posInstances.Add(1);
                    }

                    if (negativeTriple.ContainsKey(tempTriple))//  negativeTriple.find(tempTriple) != negativeTriple.end())
                    {
                        negInstances.Add(negativeTriple[tempTriple]);
                    }
                    else
                    {
                        negInstances.Add(1);
                    }
                }
                if (positiveSingle.ContainsKey(tokenized[i]))//  positiveSingle.find(tokenized[i]) != positiveSingle.end())
                {
                    posInstances.Add(positiveSingle[tokenized[i]]);
                }
                else
                {
                    posInstances.Add(1);
                }

                if (negativeSingle.ContainsKey(tokenized[i]))//  negativeSingle.find(tokenized[i]) != negativeSingle.end())
                {
                    negInstances.Add(negativeSingle[tokenized[i]]);
                }
                else
                {
                    negInstances.Add(1);
                }
            }

            //here we get the product of each positive instance we found divided by the total positive sentiment found
            double positiveNumerator = 1;
            for (int i = 0; i < posInstances.Count; i++)
            {
                positiveNumerator *= (double)posInstances[i] / (double)positiveSentimentTotal;
            }

            //here we get the product of each negative instance we found divided by the total negative sentiment found
            double negativeNumerator = 1;
            for (int i = 0; i < negInstances.Count; i++)
            {
                negativeNumerator *= (double)negInstances[i] / (double)negativeSentimentTotal;
            }

            //now multiply the found product by the fraction of the total data points
            positiveNumerator *= (double)positiveSentimentTotal / ((double)positiveSentimentTotal + (double)negativeSentimentTotal);
            negativeNumerator *= (double)negativeSentimentTotal / ((double)positiveSentimentTotal + (double)negativeSentimentTotal);

            //return true for a positive prediction, negative for negative prediction
            if (positiveNumerator > negativeNumerator)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool makeLogPrediction(List<string> input)
        {
            Tokenizer tokenizeObj = new Tokenizer();
            List<string> tokenized = input;
            List<int> posInstances = new List<int>(), negInstances = new List<int>();
            string tempTuple, tempTriple;

            //we check each token to see if it exists in the given dictionary, if it does, add the amount of instances to the vector
            //here we change 'i' depending on the start location of the text to be analyzed within the tokens
            for (int i = 0; i < tokenized.Count; i++)
            {
                //this means we have to make a tuple
                if (i < tokenized.Count - 1)
                {
                    //this makes tempTuple a way to search the dictionary for the given tuple
                    tempTuple = tokenized[i] + " " + tokenized[i + 1];
                    if (positiveTuple.ContainsKey(tempTuple))//  positiveTuple.find(tempTuple) != positiveTuple.end())
                    {
                        posInstances.Add(positiveTuple[tempTuple]);
                    }
                    else
                    {
                        posInstances.Add(1);
                    }

                    if (negativeTuple.ContainsKey(tempTuple))//  negativeTuple.find(tempTuple) != negativeTuple.end())
                    {
                        negInstances.Add(negativeTuple[tempTuple]);
                    }
                    else
                    {
                        negInstances.Add(1);
                    }
                }
                //this means make a triple
                if (i < tokenized.Count - 2 && tokenized.Count != 1)
                {
                    //this makes tempTriple a way to search the dictionary for the given tuple
                    tempTriple = tokenized[i] + " " + tokenized[i + 1] + " " + tokenized[i + 2];
                    if (positiveTriple.ContainsKey(tempTriple))//  positiveTriple.find(tempTriple) != positiveTriple.end())
                    {
                        posInstances.Add(positiveTriple[tempTriple]);
                    }
                    else
                    {
                        posInstances.Add(1);
                    }

                    if (negativeTriple.ContainsKey(tempTriple))//  negativeTriple.find(tempTriple) != negativeTriple.end())
                    {
                        negInstances.Add(negativeTriple[tempTriple]);
                    }
                    else
                    {
                        negInstances.Add(1);
                    }
                }

                if (positiveSingle.ContainsKey(tokenized[i]))//  positiveSingle.find(tokenized[i]) != positiveSingle.end())
                {
                    posInstances.Add(positiveSingle[tokenized[i]]);
                }
                else
                {
                    posInstances.Add(1);
                }

                if (negativeSingle.ContainsKey(tokenized[i]))//  negativeSingle.find(tokenized[i]) != negativeSingle.end())
                {
                    negInstances.Add(negativeSingle[tokenized[i]]);
                }
                else
                {
                    negInstances.Add(1);
                }
            }

            //here we get the summation of each positive instance we found divided by the total positive sentiment found
            double positiveNumerator = 0;
            for (int i = 0; i < posInstances.Count; i++)
            {
                positiveNumerator += Math.Log((double)posInstances[i] / (double)positiveSentimentTotal);
            }

            //here we get the summation of each negative instance we found divided by the total negative sentiment found
            double negativeNumerator = 0;
            for (int i = 0; i < negInstances.Count; i++)
            {
                negativeNumerator += Math.Log((double)negInstances[i] / (double)negativeSentimentTotal);
            }

            //now add the found product by the fraction of the total data points
            positiveNumerator += Math.Log((double)positiveSentimentTotal / ((double)positiveSentimentTotal + (double)negativeSentimentTotal));
            negativeNumerator += Math.Log((double)negativeSentimentTotal / ((double)positiveSentimentTotal + (double)negativeSentimentTotal));

            //return true for a positive prediction, negative for negative prediction
            if (positiveNumerator > negativeNumerator)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int learn(string toRead)
        {
            // Tokenize the tweets and assign them to maps
            string line = "";
            List<string> tokens;
            Tokenizer t = new Tokenizer();
            StreamReader file = new StreamReader(toRead);
            //ifstream file(toRead);
            string sentiment;

            line = file.ReadLine();
            while (line != null)
            {
                //cout << line << endl;

                tokens = t.tokenize(line);



                // Here the second element of tokens is the sentiment.  Grab it.
                sentiment = tokens[1];

                if (sentiment == "1")
                {
                    positiveSentimentTotal++;

                    // Here the tweet had positive sentiment so we add the words to the positive bins.
                    int i = 3;
                    string twice, thrice;

                    while (i < tokens.Count)
                    {
                        //positiveSingle.try_emplace(tokens[i], 0);
                        //positiveSingle[tokens[i]]++;
                        if (positiveSingle.ContainsKey(tokens[i]))
                        {
                            positiveSingle[tokens[i]]++;
                        }
                        else
                        {
                            positiveSingle[tokens[i]] = 1;
                        }


                        if ((i + 1) < tokens.Count)
                        {
                            // If there's room, grab the next two words and add them.

                            twice = tokens[i] + " " + tokens[i + 1];
                            //positiveTuple.try_emplace(twice, 0);
                            //positiveTuple[twice]++;
                            if (positiveTuple.ContainsKey(twice))
                            {
                                positiveTuple[twice]++;
                            }
                            else
                            {
                                positiveTuple[twice] = 1;
                            }

                            // Clear twice
                            twice = "";
                        }
                        if ((i + 2) < tokens.Count)
                        {
                            thrice = tokens[i] + " " + tokens[i + 1] + " " + tokens[i + 2];
                            // Here we have looked at three words so we add.
                            //positiveTriple.try_emplace(thrice, 0);
                            //positiveTriple[thrice]++;
                            if (positiveTriple.ContainsKey(thrice))
                            {
                                positiveTriple[thrice]++;
                            }
                            else
                            {
                                positiveTriple[thrice] = 1;
                            }

                            // Clear thrice
                            thrice = "";
                        }
                        i++;
                    }


                }
                else if (sentiment == "0")
                {
                    negativeSentimentTotal++;

                    // Here the tweet had positive sentiment so we add the words to the positive bins.
                    int i = 3;
                    string twice, thrice;

                    while (i < tokens.Count)
                    {
                        //negativeSingle.try_emplace(tokens[i], 0);
                        //negativeSingle[tokens[i]]++;
                        if (negativeSingle.ContainsKey(tokens[i]))
                        {
                            negativeSingle[tokens[i]]++;
                        }
                        else
                        {
                            negativeSingle[tokens[i]] = 1;
                        }

                        if ((i + 1) < tokens.Count)
                        {
                            // If there's room, grab the next two words and add them.

                            twice = tokens[i] + " " + tokens[i + 1];
                            //negativeTuple.try_emplace(twice, 0);
                            //negativeTuple[twice]++;
                            if (negativeTuple.ContainsKey(twice))
                            {
                                negativeTuple[twice]++;
                            }
                            else
                            {
                                negativeTuple[twice] = 1;
                            }

                            // Clear twice
                            twice = "";
                        }
                        if ((i + 2) < tokens.Count)
                        {
                            thrice = tokens[i] + " " + tokens[i + 1] + " " + tokens[i + 2];
                            // Here we have looked at three words so we add.
                            //negativeTriple.try_emplace(thrice, 0);
                            //negativeTriple[thrice]++;
                            if (negativeTriple.ContainsKey(thrice))
                            {
                                negativeTriple[thrice]++;
                            }
                            else
                            {
                                negativeTriple[thrice] = 1;
                            }

                            // Clear thrice
                            thrice = "";
                        }
                        i++;
                    }
                }
                line = file.ReadLine();
                //cin.ignore();
            }

            return 1;
        }

        public void saveLearningFull(int time)
        {
            TextWriter tOut = Console.Out;
            // This function will take everything that has been learned and output it to text files
            //cout << "Saving" << endl;
            tOut.WriteLine("Saving Positive Singles\n");
            saveLearningSingle(positiveSingle, "SaveData/pSingle.txt", 3);
            tOut.WriteLine("Saving Positive Tuples\n");
            saveLearningSingle(positiveTuple, "SaveData/pTuple.txt", 3);
            tOut.WriteLine("Saving Positive Triples\n");
            saveLearningSingle(positiveTriple, "SaveData/pTriple.txt", 3);

            tOut.WriteLine("Saving Negative Singles\n");
            saveLearningSingle(negativeSingle, "SaveData/nSingle.txt", 3);
            tOut.WriteLine("Saving Negative Tuples\n");
            saveLearningSingle(negativeTuple, "SaveData/nTuple.txt", 3);
            tOut.WriteLine("Saving Negative Triples\n");
            saveLearningSingle(negativeTriple, "SaveData/nTriple.txt", 3);

            // Here we save the total number of strings read in for positive and negative.
            //fstream saveTotals;
            //saveTotals.open("SaveData/totals.txt", std::fstream::out);
            StreamWriter saveTotals = new StreamWriter("SaveData/totals.txt");
            saveTotals.WriteLine(positiveSentimentTotal.ToString() + " " + negativeSentimentTotal.ToString());
            //saveTotals << positiveSentimentTotal << " " << negativeSentimentTotal;
            saveTotals.Close();
        }

        void saveLearningSingle(Dictionary<string, int> data, string fname, int occLevel)
        {
            // IMPORTANT:::: the occLevel is the level at which the occurence has to reach in order to be saved.
            //fstream saveFile;
            //saveFile.open(fname, std::fstream::out);
            StreamWriter saveFile = new StreamWriter(fname);

            /*for (keyValue : data)
            {
                if (keyValue.second > occLevel)
                {
                    saveFile << keyValue.first << " " << keyValue.second << endl;
                }

            }*/

            foreach (var keyValue in data)
            {
                if (keyValue.Value > occLevel)
                {
                    saveFile.WriteLine(keyValue.Key.ToString() + " " + keyValue.Value.ToString());
                }
            }
            saveFile.Close();
        }

        public void learnFull()
        {
            string response;
            string current;
            string learn = "Learning/";
            string line;
            int i = 1, currentI = 0;
            TextReader tIn = Console.In;
            TextWriter tOut = Console.Out;

            tOut.WriteLine("Warning: This function will begin learning from the full archive of 1.5 million tweets and will take an estimated 3 hours to execute.  Are you sure you want to do this? (Y/N)\n");
            response = tIn.ReadLine();

            while (response != "y" && response != "Y")
            {
                if (response == "n" || response == "N")
                {
                    return;
                }
                response = tIn.ReadLine();
            }


            while (currentI <= 1570000)
            {
                // Calculate the name of the file.
                currentI += 10000;

                current = currentI.ToString();//std::to_string(currentI);
                current += ".txt";
                learn += current;

                tOut.WriteLine("Learning new file: " + learn);

                //cout << "Learning new file: " << learn << endl;

                // Here we learn from the file
                this.learn(learn);

                //Reset the learn string
                learn = "Learning/";

                // This if statement makes it so it saves its progress every 100,000 tweets.
                if (currentI % 100000 == 0)
                {
                    tOut.WriteLine("Saving progress.\n");
                    this.saveLearningFull(currentI / 100000);
                    positiveSingle.Clear();
                    positiveTuple.Clear();
                    positiveTuple.Clear();
                    negativeSingle.Clear();
                    negativeTuple.Clear();
                    negativeTriple.Clear();
                    this.loadLearningFull();
                }
            }
            this.saveLearningFull(currentI / 100000);
        }

        public void loadLearningFull()
        {
            TextWriter tOut = Console.Out;

            tOut.WriteLine("Loading Positive Single\n");
            positiveSingle = loadLearningSingle("pSingle.txt");
            tOut.WriteLine("Loading Positive Tuple\n");
            positiveTuple = loadLearningSingle("pTuple.txt");
            tOut.WriteLine("Loading Positive Triple\n");
            positiveTriple = loadLearningSingle("pTriple.txt");

            tOut.WriteLine("Loading Negative Single\n");
            negativeSingle = loadLearningSingle("nSingle.txt");
            tOut.WriteLine("Loading Negative Tuple\n");
            negativeTuple = loadLearningSingle("nTuple.txt");
            tOut.WriteLine("Loading Negative Triple\n");
            negativeTriple = loadLearningSingle("nTriple.txt");

            tOut.WriteLine("Loading Totals\n");
            loadTotals();
        }

        public Dictionary<string, int> loadLearningSingle(string fname)
        {
            // Load data is an embedded resource now
            Assembly assembly = Assembly.GetExecutingAssembly();
            StreamReader loadFile = new StreamReader(assembly.GetManifestResourceStream($@"Sentiment.Analyzer.Resources.{fname}"));
            
            string line;
            string toAdd;
            string occ;
            int pos;

            Dictionary<string, int> loadInto = new Dictionary<string, int>();

            //loadFile.open(fname, std::fstream::in);
            line = loadFile.ReadLine();
            while (line != null)
            {
                pos = line.LastIndexOf(" ");
                //pos = line.find_last_of(" ");
                toAdd = line.Substring(0, pos);
                //toAdd = line.substr(0, pos);
                occ = line.Substring(pos + 1);
                //occ = line.substr(pos + 1);
                try
                {
                    loadInto.Add(toAdd, Convert.ToInt32(occ));
                }
                catch
                {
                    //
                }

                line = loadFile.ReadLine();
                //loadInto.emplace(toAdd, stoi(occ));
            }

            return loadInto;
        }

        public void loadTotals()
        {
            string line;
            int pos;

            // Load data is an embedded resource now
            Assembly assembly = Assembly.GetExecutingAssembly();
            StreamReader totals = new StreamReader(assembly.GetManifestResourceStream($@"Sentiment.Analyzer.Resources.totals.txt"));

            //getline(totals, line);
            line = totals.ReadLine();
            pos = line.IndexOf(' ');
            positiveSentimentTotal = Convert.ToInt32(line.Substring(0, pos));
            negativeSentimentTotal = Convert.ToInt32(line.Substring(pos + 1));

            totals.Close();
        }
    }
}
