using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tgif_clarifi
{
    public class Tag
    {
        string name;
        double probability;

        public Tag(string name, double probability)
        {
            this.name = name;
            this.probability = probability;
        }

        /**
         * Returns the name of the tag. The name will be in the language specified by the Locale passed
         * to {@link RecognitionRequest#setLocale(java.util.Locale)}, or the application's default
         * language if none was specified. Note that the name may consist of more than one word.
         */
        public string getName()
        {
            return name;
        }

        /** Returns a probability that this tag is associated with the input image. */
        public double getProbability()
        {
            return probability;
        }

        public override string ToString()
        {
            return "[" + name + ": " + probability + "]";
        }
    }
}
