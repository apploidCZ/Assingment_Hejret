using System.Text;

namespace DNANexus;

// Preconditions: dotnet installed
// Run the program by opening the terminal and running dotnet run "<filePath>" "<L-number>"
class Program
{
    // Method for program execution
    static void Main(string[] args)
    {

        // If user passed less than two arguments, end program by displaying warning message
        if (args.Length < 2) {
            Console.WriteLine("Please enter valid input: dotnet run <pathToInputFileIn> <the number L>");
            Console.WriteLine("Example input: dotnet run \"D:\\PathToFile\\file\" \"6\"");
        }

        // If user passed at least two arguments, start parsing
        else {
            int elNumber = Convert.ToInt32(args[1]);
            string filePathToInputFile = args[0];
            byte[] fileBytes = File.ReadAllBytes(filePathToInputFile);

            // Create StringBuilder objects to store the DNA letters and confidence score later
            // Stringbuilder objects should be faster than normal strings
            StringBuilder dnaLetters = new StringBuilder();
            StringBuilder confidenceScores = new StringBuilder();

            // Counter for tracking the number of bytes and pieces
            int byteCounter = 1;
            int pieceCounter = 1;

            // The loop will transform each byte into a single letter and confidence score
            // Once the DNA piece is stringed together depending on the L number, it outputs the result
            foreach (byte b in fileBytes)
            {
                // Convert the byte b into single bits
                // Add leading zeros to the binary string, if necessary
                string byteInBinary = Convert.ToString(b, 2).PadLeft(8, '0');

                // Transform the byte and append it into the Stringbuilder objects
                dnaLetters.Append(GetDnaLetter(byteInBinary));
                confidenceScores.Append(GetConfidenceScore(byteInBinary));

                /* 
                The length of the piece is given by the L number
                To get the correct piece size, we print out the results every x-th time, where x is the L number. 
                We accomplish this by dividing the number of processed bytes and the given L number
                If the division reminder is zero, we print out the result 
                */
                if (byteCounter % elNumber == 0)
                {
                    Console.WriteLine("@READ_" + pieceCounter);
                    Console.WriteLine(dnaLetters.ToString());
                    Console.WriteLine("+READ_" + pieceCounter);
                    Console.WriteLine(confidenceScores.ToString());

                    // Clear the StringBuilder objects for the next DNA piece
                    dnaLetters.Clear();
                    confidenceScores.Clear();
                    pieceCounter++;
                }
                byteCounter++;
            }
        }
    }

    // Input is a byte in it's binary representation
    static string GetDnaLetter(string byteInBinary) {

        // Get first two bits to determine the DNA letter
        string dnaLetterInBits = byteInBinary.Substring(0,2);
        if (dnaLetterInBits == "00") {
            return "A";
        }
        else if (dnaLetterInBits == "01") {
            return "C";
        }
        else if (dnaLetterInBits == "11") {
            return "T";
        }
        else if (dnaLetterInBits == "10") {
            return "G";
        }

        // Define some fallback values for debugging purposes
        else {
            return "F";
        }
    }

    // Input is a byte in it's binary representation
    static string GetConfidenceScore(string byteInBinary) {

        // Get the last six bits to determine confidence score
        string score = byteInBinary.Substring(2, 6);

        // Convert (base 2) bits to int
        int transformedScore = Convert.ToInt32(score, 2);

        // Add 33 to integer is in valid ascii range
        transformedScore += 33;

        // If score in 33 a 96 range, we know it will be converted to a valid ascii char
        if (transformedScore >= 33 && transformedScore <= 96) {
            return Convert.ToChar(transformedScore).ToString();
        }
        
        // Define a fall back value in case score is not valid
        else {
            return "¥";
        }
    }
}
