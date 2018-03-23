using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
//using System.IO;
//using System.Reflection;
using Sentiment.Web.Api.Models;
using Sentiment.Common;

namespace Sentiment.Analyzer
{
    public class FastText
    {

        public FastText()
        {
            labelList = new List<string>();
        }

        public List<string> labelList;
        public List<int> getLabels()
        {
            List<int> toRet = new List<int>();
            foreach (var label in labelList)
            {
                toRet.Add(int.Parse(label[9].ToString()));
            }
            labelList.Clear();
            return toRet;
        }

        public void runPrediction(string inputFileName)
        {
            var fastTextPath = ResourceHelper.ExtractResource("Sentiment.Analyzer.Resources.fastText.exe");
            var modelPath = ResourceHelper.ExtractResource("Sentiment.Analyzer.Resources.model.ftz");

            ExeHelper exeHelper = new ExeHelper();
            var output = exeHelper.Run(fastTextPath, $"predict {modelPath} {inputFileName}");
            var outLines = output
                .Split('\n', '\r')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();

            //ProcessStartInfo start = new ProcessStartInfo();
            //Process fastText = new Process();

            ////start.CreateNoWindow = false;
            //start.UseShellExecute = false;
            //start.FileName = fastTextPath;
            //start.WindowStyle = ProcessWindowStyle.Hidden;
            //start.Arguments = $"predict {modelPath} {inputFileName}";
            //start.RedirectStandardOutput = true;
            //start.CreateNoWindow = false;
            //start.UseShellExecute = false;
            //fastText.StartInfo = start;

            //fastText.Start();
            //string[] output = fastText.StandardOutput
            //    .ReadToEnd()
            //    .Split('\n', '\r')
            //    .Where(s => !string.IsNullOrWhiteSpace(s))
            //    .ToArray();

            foreach (var line in outLines)
            {
                labelList.Add(line);
            }
            //while (!fastText.StandardOutput.EndOfStream)
            //{
            //    labelList.Add(fastText.StandardOutput.ReadLine());
            //}
            //fastText.WaitForExit();


            foreach (var line in labelList)
            {
                Console.Write("List output: " + line + "\n");
            }

            /*try
            {
                using (Process x = Process.Start(start))
                {
                    x.WaitForExit();
                    output = x.StandardOutput.ReadToEnd();
                    Console.Write("Process returned: " + output);
                }
            }
            catch
            {

            }*/
        }
    }

    public class tokenizer
    {
        public List<string> tokenize(string input)
        {
            List<string> toRet = new List<string>();
            string token = "";

            input = input.ToLower();

            for (int i = 0; i < input.Length; i++)
            {
                if (this.tokenizable(input[i]))
                {
                    if (token != "")
                    {
                        toRet.Add(token);
                        token = "";
                    }
                }
                else
                {
                    token += input[i];
                }
            }

            if (token != "")
            {
                toRet.Add(token);
            }

            this.preProcess(ref toRet);

            return toRet;
        }

        public bool tokenizable(char c)
        {
            string check = " .,/?;:[]{}!+=-_()*~%^|\"";

            for (int i = 0; i < check.Length; i++)
            {
                if (check[i] == c)
                {
                    return true;
                }
            }

            return false;
        }

        public void preProcess(ref List<string> toProcess)
        {
            // This function makes alterations to the token string.  Different ideas will be tried here,
            // so if you make a change here COMMENT WHAT IT DOES



            // This loop looks for "not" and takes the next word and appends it
            for (int i = 0; i < toProcess.Count; i++)
            {
                if (toProcess[i] == "not")
                {
                    // Check to make sure its not the last word
                    if (i < toProcess.Count - 1)
                    {
                        //the commented out code here is the c++ equivalent, I kept it just in case
                        //I didn't translate it to c# correctly
                        toProcess[i] = toProcess[i] + toProcess[i + 1];
                        //toProcess->at(i).append(toProcess->at(i + 1));
                        toProcess.RemoveAt(i + 1);
                        //toProcess->erase(toProcess->begin() + i + 1);
                    }
                }
            }
        }
    }

    //in order to run the sentiment analyizer, make sure to have the SaveData file
    //then run the function loadLearningFull that is part of the NaiveBayes class
    //
    public class NaiveBayesAndFT : ISentimentAnalyzer
    {
        Dictionary<string, int> positiveSingle;
        Dictionary<string, int> positiveTuple;
        Dictionary<string, int> positiveTriple;

        Dictionary<string, int> negativeSingle;
        Dictionary<string, int> negativeTuple;
        Dictionary<string, int> negativeTriple;

        int negativeSentimentTotal;
        int positiveSentimentTotal;
        FastText runFastText;

        public NaiveBayesAndFT()
        {
            runFastText = new FastText();
            loadLearningFull();
        }

        public Dictionary<DateTime, SentimentInfo> GetSentiment(IEnumerable<Tweet> tweets, TimeLibrary.TimeInterval groupingInterval)
        {
            // If there are no tweets, return empty Dictionary
            if (tweets.Any() == false) return new Dictionary<DateTime, SentimentInfo>();

            var sentimentScores = new Dictionary<DateTime, SentimentInfo>();
            //List<string> groupTweets = new List<string>();
            //List<bool> groupSentiment = new List<bool>();
            //// This will group the tweets by hour
            //var tweetsGroupedByHour = tweets
            //    .GroupBy(t => new DateTime(t.CreatedAt.Year, t.CreatedAt.Month, t.CreatedAt.Day, t.CreatedAt.Hour, 0, 0));

            var scores = this.runPrediction(tweets.Select(t => t.Text).ToList());

            var tweetsWithScore = tweets.Zip(scores, (t, s) => new { Score = s, Tweet = t });


            // This will group the tweets by month
            var tweetsGrouped = tweetsWithScore.GroupByTime(t => t.Tweet.CreatedAt, groupingInterval);
            //    .GroupBy(t => new DateTime(t.Tweet.CreatedAt.Year, t.Tweet.CreatedAt.Month, 1, 0, 0, 0));


            // foreach group of tweets
            foreach (var tweetGroup in tweetsGrouped)
            {
                var numberOfTweets = tweetGroup.Count();

                //// calculate the sum of sentiment of each tweet in the group
                var sentiment = 0.00;

                var groupSentiment = tweetGroup.Select(g => g.Score);

                foreach(var prediction in groupSentiment)
                {
                    //prediction being set to true means that a positive prediction was made
                    if(prediction == true)
                    {
                        sentiment += 1;
                    }
                }

                sentiment /= numberOfTweets;

                // add the datetime and the sentiment info to the sentiment dictionary
                sentimentScores
                    .Add(
                        tweetGroup.Key,
                        new SentimentInfo()
                        {
                            Sentiment = sentiment,
                            Weight = numberOfTweets
                        });
            }

            return sentimentScores;
        }

        public List<bool> runPrediction(List<string> toPredict)
        {
            List<bool> ToRet = new List<bool>();
            tokenizer t = new tokenizer();
            List<string> tokens;
            int retValue;
            List<int> retVals = new List<int>();
            List<string> badTweets = new List<string>();
            
            foreach (var tweet in toPredict)
            {
                tokens = t.tokenize(tweet);
                retValue = makeLogPrediction(tokens);
                retVals.Add(retValue);
                
                //if the returned value for the tweet says that we did not have sufficient evidence to make a prediction, add it to this list
                if (retValue == -1)
                {
                    badTweets.Add(tweet);
                }
            }
            
            //create file for fastText to read from
            var badTweetsPath = ResourceHelper.WriteToTemp("badtweets.txt", badTweets);
            
            if (badTweets.Count > 0)
            {
                Console.WriteLine("Running fastText on: " + badTweets.Count.ToString() + " Tweets");
                runFastText.runPrediction(badTweetsPath);
                int listCount = 0;
                List<int> badLabels = runFastText.getLabels();
                
                int tester = 0;

                for (int i = 0; i < retVals.Count; i++)
                {
                    tester = 1;

                    try
                    {
                        tester = 2;

                        if (retVals[i] == -1)
                        {
                            tester = 3;

                            retVals[i] = badLabels[listCount]; // badLabels.count == 0. FT isn't saving right

                            tester = 4;

                            listCount++;
                        }

                        tester = 5;


                        if (retVals[i] == 1)
                        {
                            tester = 6; 

                            ToRet.Add(true);
                        }
                        else
                        {
                            tester = 7;

                            ToRet.Add(false);
                        }

                        tester = 8;
                    }
                    catch (Exception x)
                    {
                        string message = $@"
tester={tester}
i={i}
listCount={listCount}
retVal.Count={retVals.Count}
badLabels.Count={badLabels.Count}";

                        throw new Exception(message, x);
                    }
                }
            }
            
            return ToRet;
        }

        //returns true for a positive prediction, negative for negative prediction
        public bool makePrediction(string input)
        {
            tokenizer tokenizeObj = new tokenizer();
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

        //returns 1 for a positive prediciton 0 for negative prediction, and -1 if there is not enough certainty
        public int makeLogPrediction(List<string> input)
        {
            tokenizer tokenizeObj = new tokenizer();
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

            if (Math.Abs(positiveNumerator - negativeNumerator) < 2)
            {
                return -1;
            }

            //return true for a positive prediction, negative for negative prediction
            if (positiveNumerator > negativeNumerator)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        //public void saveLearningFull(int time)
        //{
        //    TextWriter tOut = Console.Out;
        //    // This function will take everything that has been learned and output it to text files
        //    //cout << "Saving" << endl;
        //    tOut.WriteLine("Saving Positive Singles\n");
        //    saveLearningSingle(positiveSingle, "SaveData/pSingle.txt", 3);
        //    tOut.WriteLine("Saving Positive Tuples\n");
        //    saveLearningSingle(positiveTuple, "SaveData/pTuple.txt", 3);
        //    tOut.WriteLine("Saving Positive Triples\n");
        //    saveLearningSingle(positiveTriple, "SaveData/pTriple.txt", 3);

        //    tOut.WriteLine("Saving Negative Singles\n");
        //    saveLearningSingle(negativeSingle, "SaveData/nSingle.txt", 3);
        //    tOut.WriteLine("Saving Negative Tuples\n");
        //    saveLearningSingle(negativeTuple, "SaveData/nTuple.txt", 3);
        //    tOut.WriteLine("Saving Negative Triples\n");
        //    saveLearningSingle(negativeTriple, "SaveData/nTriple.txt", 3);

        //    // Here we save the total number of strings read in for positive and negative.
        //    //fstream saveTotals;
        //    //saveTotals.open("SaveData/totals.txt", std::fstream::out);
        //    StreamWriter saveTotals = new StreamWriter("SaveData/totals.txt");
        //    saveTotals.WriteLine(positiveSentimentTotal.ToString() + " " + negativeSentimentTotal.ToString());
        //    //saveTotals << positiveSentimentTotal << " " << negativeSentimentTotal;
        //    saveTotals.Close();
        //}

        //void saveLearningSingle(Dictionary<string, int> data, string fname, int occLevel)
        //{
        //    // IMPORTANT:::: the occLevel is the level at which the occurence has to reach in order to be saved.
        //    //fstream saveFile;
        //    //saveFile.open(fname, std::fstream::out);
        //    StreamWriter saveFile = new StreamWriter(fname);

        //    /*for (keyValue : data)
        //    {
        //        if (keyValue.second > occLevel)
        //        {
        //            saveFile << keyValue.first << " " << keyValue.second << endl;
        //        }

        //    }*/

        //    foreach (var keyValue in data)
        //    {
        //        if (keyValue.Value > occLevel)
        //        {
        //            saveFile.WriteLine(keyValue.Key.ToString() + " " + keyValue.Value.ToString());
        //        }
        //    }
        //    saveFile.Close();
        //}

        //public void learnFull()
        //{
        //    string response;
        //    string current;
        //    string learn = "Learning/";
        //    int currentI = 0;
        //    TextReader tIn = Console.In;
        //    TextWriter tOut = Console.Out;

        //    tOut.WriteLine("Warning: This function will begin learning from the full archive of 1.5 million tweets and will take an estimated 3 hours to execute.  Are you sure you want to do this? (Y/N)\n");
        //    response = tIn.ReadLine();

        //    while (response != "y" && response != "Y")
        //    {
        //        if (response == "n" || response == "N")
        //        {
        //            return;
        //        }
        //        response = tIn.ReadLine();
        //    }


        //    while (currentI <= 1570000)
        //    {
        //        // Calculate the name of the file.
        //        currentI += 10000;

        //        current = currentI.ToString();//std::to_string(currentI);
        //        current += ".txt";
        //        learn += current;

        //        tOut.WriteLine("Learning new file: " + learn);

        //        //cout << "Learning new file: " << learn << endl;

        //        // Here we learn from the file
        //        this.learn(learn);

        //        //Reset the learn string
        //        learn = "Learning/";

        //        // This if statement makes it so it saves its progress every 100,000 tweets.
        //        if (currentI % 100000 == 0)
        //        {
        //            tOut.WriteLine("Saving progress.\n");
        //            this.saveLearningFull(currentI / 100000);
        //            positiveSingle.Clear();
        //            positiveTuple.Clear();
        //            positiveTuple.Clear();
        //            negativeSingle.Clear();
        //            negativeTuple.Clear();
        //            negativeTriple.Clear();
        //            this.loadLearningFull();
        //        }
        //    }
        //    this.saveLearningFull(currentI / 100000);
        //}

        //public int learn(string toRead)
        //{
        //    // Tokenize the tweets and assign them to maps
        //    string line = "";
        //    List<string> tokens;
        //    tokenizer t = new tokenizer();
        //    StreamReader file = new StreamReader(toRead);
        //    //ifstream file(toRead);
        //    string sentiment;

        //    line = file.ReadLine();
        //    while (line != null)
        //    {
        //        //cout << line << endl;

        //        tokens = t.tokenize(line);



        //        // Here the second element of tokens is the sentiment.  Grab it.
        //        sentiment = tokens[1];

        //        if (sentiment == "1")
        //        {
        //            positiveSentimentTotal++;

        //            // Here the tweet had positive sentiment so we add the words to the positive bins.
        //            int i = 3;
        //            string twice, thrice;

        //            while (i < tokens.Count)
        //            {
        //                //positiveSingle.try_emplace(tokens[i], 0);
        //                //positiveSingle[tokens[i]]++;
        //                if (positiveSingle.ContainsKey(tokens[i]))
        //                {
        //                    positiveSingle[tokens[i]]++;
        //                }
        //                else
        //                {
        //                    positiveSingle[tokens[i]] = 1;
        //                }


        //                if ((i + 1) < tokens.Count)
        //                {
        //                    // If there's room, grab the next two words and add them.

        //                    twice = tokens[i] + " " + tokens[i + 1];
        //                    //positiveTuple.try_emplace(twice, 0);
        //                    //positiveTuple[twice]++;
        //                    if (positiveTuple.ContainsKey(twice))
        //                    {
        //                        positiveTuple[twice]++;
        //                    }
        //                    else
        //                    {
        //                        positiveTuple[twice] = 1;
        //                    }

        //                    // Clear twice
        //                    twice = "";
        //                }
        //                if ((i + 2) < tokens.Count)
        //                {
        //                    thrice = tokens[i] + " " + tokens[i + 1] + " " + tokens[i + 2];
        //                    // Here we have looked at three words so we add.
        //                    //positiveTriple.try_emplace(thrice, 0);
        //                    //positiveTriple[thrice]++;
        //                    if (positiveTriple.ContainsKey(thrice))
        //                    {
        //                        positiveTriple[thrice]++;
        //                    }
        //                    else
        //                    {
        //                        positiveTriple[thrice] = 1;
        //                    }

        //                    // Clear thrice
        //                    thrice = "";
        //                }
        //                i++;
        //            }


        //        }
        //        else if (sentiment == "0")
        //        {
        //            negativeSentimentTotal++;

        //            // Here the tweet had positive sentiment so we add the words to the positive bins.
        //            int i = 3;
        //            string twice, thrice;

        //            while (i < tokens.Count)
        //            {
        //                //negativeSingle.try_emplace(tokens[i], 0);
        //                //negativeSingle[tokens[i]]++;
        //                if (negativeSingle.ContainsKey(tokens[i]))
        //                {
        //                    negativeSingle[tokens[i]]++;
        //                }
        //                else
        //                {
        //                    negativeSingle[tokens[i]] = 1;
        //                }

        //                if ((i + 1) < tokens.Count)
        //                {
        //                    // If there's room, grab the next two words and add them.

        //                    twice = tokens[i] + " " + tokens[i + 1];
        //                    //negativeTuple.try_emplace(twice, 0);
        //                    //negativeTuple[twice]++;
        //                    if (negativeTuple.ContainsKey(twice))
        //                    {
        //                        negativeTuple[twice]++;
        //                    }
        //                    else
        //                    {
        //                        negativeTuple[twice] = 1;
        //                    }

        //                    // Clear twice
        //                    twice = "";
        //                }
        //                if ((i + 2) < tokens.Count)
        //                {
        //                    thrice = tokens[i] + " " + tokens[i + 1] + " " + tokens[i + 2];
        //                    // Here we have looked at three words so we add.
        //                    //negativeTriple.try_emplace(thrice, 0);
        //                    //negativeTriple[thrice]++;
        //                    if (negativeTriple.ContainsKey(thrice))
        //                    {
        //                        negativeTriple[thrice]++;
        //                    }
        //                    else
        //                    {
        //                        negativeTriple[thrice] = 1;
        //                    }

        //                    // Clear thrice
        //                    thrice = "";
        //                }
        //                i++;
        //            }
        //        }
        //        line = file.ReadLine();
        //        //cin.ignore();
        //    }

        //    return 1;
        //}

        public void loadLearningFull()
        {
            //TextWriter tOut = Console.Out;

            //tOut.WriteLine("Loading Positive Single\n");
            positiveSingle = loadLearningSingle("pSingle.txt");
            //tOut.WriteLine("Loading Positive Tuple\n");
            positiveTuple = loadLearningSingle("pTuple.txt");
            //tOut.WriteLine("Loading Positive Triple\n");
            positiveTriple = loadLearningSingle("pTriple.txt");

            //tOut.WriteLine("Loading Negative Single\n");
            negativeSingle = loadLearningSingle("nSingle.txt");
            //tOut.WriteLine("Loading Negative Tuple\n");
            negativeTuple = loadLearningSingle("nTuple.txt");
            //tOut.WriteLine("Loading Negative Triple\n");
            negativeTriple = loadLearningSingle("nTriple.txt");

            //tOut.WriteLine("Loading Totals\n");
            loadTotals();
        }

        public Dictionary<string, int> loadLearningSingle(string fname)
        {
            // Load data is an embedded resource now
            //Assembly assembly = Assembly.GetExecutingAssembly();
            //StreamReader loadFile = new StreamReader(assembly.GetManifestResourceStream($@"Sentiment.Analyzer.Resources.{fname}"));

            var lines = ResourceHelper.ReadResourceString($@"Sentiment.Analyzer.Resources.{fname}").Split('\n');
            
            string toAdd;
            string occ;
            int pos;

            Dictionary<string, int> loadInto = new Dictionary<string, int>();
            
            foreach(var line in lines)
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
            }
            return loadInto;
        }

        public void loadTotals()
        {
            // Load data is an embedded resource now
            var line = ResourceHelper.ReadResourceString($@"Sentiment.Analyzer.Resources.totals.txt");

            var pos = line.IndexOf(' ');
            positiveSentimentTotal = Convert.ToInt32(line.Substring(0, pos));
            negativeSentimentTotal = Convert.ToInt32(line.Substring(pos + 1));
        }
    }
}
