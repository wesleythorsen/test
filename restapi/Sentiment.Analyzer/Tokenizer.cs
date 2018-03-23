using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Analyzer
{
    public class Tokenizer
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
}
