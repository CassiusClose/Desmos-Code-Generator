using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesmosCodeGen
{
    /// <summary>
    /// This tab generates Desmos code for a multiple choice question that displays
    /// messages about the correctness of the user's choices.
    /// </summary>
    public partial class Tab1 : UserControl
    {
        public Tab1()
        {
            InitializeComponent();
        }
        
        /*
         * Generates Desmos code from the inputted options and displays it in the code textbox
         */
        private void UpdateCode(object sender, RoutedEventArgs e)
        {
            // Reset code textbox
            resultText.Text = "";

            int i;
            int len = optionsList.Children.Count;

            // Compile lists of options: all, correct, incorrect answers
            List<int> all = new List<int>();
            List<int> correct = new List<int>();
            List<int> incorrect = new List<int>();

            for (i = 0; i < len; i++)
            {
                if (((OptionCheckbox)optionsList.Children[i]).GetCheck())
                    correct.Add(i);
                else
                    incorrect.Add(i);
                all.Add(i);
            }

            /**** Initialization Section ****/

            // Initialize choice variable in Desmos
            String choiceName = Option_ChoiceName.Text;
            resultText.Text += "choice = " + choiceName + "\n\n";


            // Initialize option variables in Desmos
            resultText.Text += "# Variables for each option\n";
            for (i = 0; i < len; i++)
                resultText.Text += IntToLetter(i) + "=choice.isSelected(" + (i + 1) + ")\n";


            // Get messages from the user input
            String message_NoneSelected = Option_NoSelections.Text;
            String message_NoneCorrect = Option_NoCorrect.Text;
            String message_SomeCorrect_NoIncorrect = Option_IncompleteCorrect.Text;
            String message_SomeCorrect_SomeIncorrect = Option_SomeCorrect_SomeIncorrect.Text;
            String message_AllCorrect = Option_AllCorrect.Text;
            /*
             * Default:
             * None selected: You must select at least one answer. Try again.
             * No correct: None of your answers are correct. Try again
             * Fewer than all correct, no incorrect: You chose X correct answer(s), but that is not the only one/those are not the only ones. Try again.
             * Some correct + some incorrect: Only X of your answers is/are correct. Try again.
             * All correct + no incorrect: You selected all the correct choices!
             */

            // Initialize variables that hold boolean statments involving the choices in Desmos
            String all_or = CreateOrString(all);
            String correct_or = CreateOrString(correct);
            String correct_and = CreateAndString(correct);
            String incorrect_or = CreateOrString(incorrect);

            resultText.Text += "all_or = " + all_or + "\n";
            resultText.Text += "correct_or = " + correct_or + "\n";
            resultText.Text += "correct_and = " + correct_and + "\n";
            resultText.Text += "incorrect_or = " + incorrect_or + "\n";
            resultText.Text += "\n";



            /*
             * ---Boolean statements for each case---
             * none selected: not(all_or)
             * no correct: not(correct_or)
             * some correct + some incorrect:
             *      for all possible sets:
             *      incorrect_or and chosen_and

             * fewer than all correct, no incorrect:
             *      for all possible sets:
             *          chosen_and and not(notchosen_or) and not(incorrect_or)
             * 
             * All correct + no incorrect: correct_and and not(incorrect_or)
             */
            
            /**** Content Section ****/

            String message_CheckSubmitted = "when choice.submitted and";
            resultText.Text += "\n\ncontent:\n";

            // No choices selected
            resultText.Text += "# No choices selected\n";
            resultText.Text += message_CheckSubmitted + "(not(all_or))" + "\"" + message_NoneSelected + "\"\n\n";

            // No correct answers selected
            resultText.Text += "# No correct answers selected\n";
            resultText.Text += message_CheckSubmitted + "(not(correct_or))" + "\"" + message_NoneCorrect + "\"\n\n";

            // Get all possible combinations of the correct answers, sort them by size (largest first)
            // Sort because Desmos treats everything as else if, so we want the biggest cases first
            List<List<int>> comb = Combinations(correct);
            comb = SortByLen(comb);

            // Some correct answers, some incorrect
            resultText.Text += "# Some number of correct answers selected, but incorrect answers also selected\n";
            for(i = 0; i < comb.Count; i++)
            {
                String chosen_and = CreateAndString(comb[i]);

                String plural_tobe = "are";
                if (comb[i].Count == 1)
                    plural_tobe = "is";

                resultText.Text += message_CheckSubmitted + "(incorrect_or and " + chosen_and + ")" +
                    "\"" + String.Format(message_SomeCorrect_SomeIncorrect, comb[i].Count, plural_tobe) + "\"\n";
            }
            resultText.Text += "\n";



            // No incorrect, but not all correct answers
            resultText.Text += "# No incorrect answers selected, but not all correct answers selected\n";
            // The combinations are sorted by largest to smallest size, so skip first one
            // because it's the correct case (all correct answers selected)
            for (i = 1; i < comb.Count; i++)
            {
                List<int> notchosen = ArrayDiff(all, comb[i]);
                String chosen_and = CreateAndString(comb[i]);
                String notchosen_or = CreateOrString(notchosen);

                
                String plural_s = "s";
                String plural_group = "those are";
                if (comb[i].Count == 1)
                {
                    plural_s = "";
                    plural_group = "that is";
                }

                resultText.Text += message_CheckSubmitted + "(" + chosen_and + " and not(" + notchosen_or + ") and not(incorrect_or))" +
                    "\"" + String.Format(message_SomeCorrect_NoIncorrect, comb[i].Count, plural_s, plural_group) + "\"\n";
            }
            resultText.Text += "\n";


            // Correct answers selected, no incorrect answers
            resultText.Text += "# Completely correct\n";
            resultText.Text += message_CheckSubmitted + "(correct_and and not(incorrect_or))" +
                "\"" + message_AllCorrect + "\"\n\n";


            // Otherwise, should presumably never be reached
            resultText.Text += "otherwise \"\"";
        }

        /*
         * Sorts the list of lists by decreasing size of each list element.
         */
        private List<List<int>> SortByLen(List<List<int>> l)
        {
            int j;
            for(int i = 0; i < l.Count; i++)
            {
                List<int> key = l[i];
                j = i - 1;

                while(j >= 0 && l[j].Count < key.Count)
                {
                    l[j + 1] = l[j];
                    j = j - 1;
                }
                l[j + 1] = key;

            }

            return l;
        }

        /*
         * Calculates the difference between two sets stored in lists.
         * Like set notation: a-b
         */
        private List<int> ArrayDiff(List<int> a, List<int> b)
        {
            List<int> diff = new List<int>();
            for(int i = 0; i < a.Count; i++)
            {
                bool found = false;
                for(int j = 0; j < b.Count; j++)
                {
                    if (a[i] == b[j])
                        found = true;
                }

                if (!found)
                    diff.Add(a[i]);
            }
            return diff;
        }


        /*
         * Takes a list of integers, converts them to their corresponding option letters,
         * and returns a string of them "and'ed" together in Demos Computation Layer syntax.
         */
        private String CreateAndString(List<int> l)
        {
            String result = "(";
            for(int i = 0; i < l.Count; i++)
            {
                result += IntToLetter(l[i]);
                if (i != l.Count - 1)
                    result += " and ";
            }
            result += ")";
            return result;
        }

        /*
         * Takes a list of integers, converts them to their corresponding option letters,
         * and returns a string of them "or'ed" together in Demos Computation Layer syntax.
         */
        private String CreateOrString(List<int> l)
        {
            String result = "(";
            for (int i = 0; i < l.Count; i++)
            {
                result += IntToLetter(l[i]);
                if (i != l.Count - 1)
                    result += " or ";
            }
            result += ")";
            return result;
        }

        /*
         * Returns a List of all the combinations of a given list of integers.
         * Each combination is its own List of ints.
         * Taken from Matthew's answer here:
         * https://stackoverflow.com/questions/5162254/all-possible-combinations-of-an-array
         * 
         */
        private List<List<int>> Combinations(List<int> l)
        {
            List<List<int>> comb = new List<List<int>>();
            int count = l.Count;
            
            for (long i = 1; i < Math.Pow(2, count); i++)
            {
                List<int> newl = new List<int>();
                for(int j = 0; j < l.Count; j++)
                {
                    if( (i & (long) Math.Pow(2, j)) > 0 )
                    {
                        newl.Add(l[j]);
                    }
                }
                comb.Add(newl);
            }
            return comb;
        }

        /*
         * Returns the letter associated with each integer, like those you'd see
         * on a multiple choice question. Starts at 0.
         * So 0 returns 'A', 1 returns 'B', etc.
         */
        private char IntToLetter(int i)
        {
            return (char) (i + 65);
        }

        /*
         * When the number of options is changed by the user, add or remove so that  the appropriate number of
         * OptionCheckbox objects are displayed.
         */
        private void OptionsNumChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.optionsList != null)
            {
                int newVal = (int)e.NewValue;
                int oldVal = (int)e.OldValue;

                if(newVal == oldVal)
                {
                    return;
                }
                else if(newVal < oldVal)
                {
                    int len = this.optionsList.Children.Count;
                    Console.WriteLine(len - 1);
                    Console.WriteLine(oldVal - newVal);
                    for(int i = len-1; i > (len-1)-(oldVal-newVal); i--)
                    {
                        this.optionsList.Children.RemoveAt(i);
                    }
                }
                else
                {
                    for(int i = 0; i < newVal-oldVal; i++)
                    {
                        OptionCheckbox c = new OptionCheckbox(this.optionsList.Children.Count + i);
                        this.optionsList.Children.Add(c);
                    }
                }
            }
        }
    }
}
