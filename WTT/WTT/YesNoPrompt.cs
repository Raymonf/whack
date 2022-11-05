using System;

namespace WhackTranslationTool
{
    internal class YesNoPrompt
    {
        /// <summary>
        /// Call promptMessageFunc and ask for response
        /// Default is always Y with no response
        /// </summary>
        /// <param name="promptMessageFunc">Optional function to call before ReadLine()</param>
        /// <returns>The answer</returns>
        public static bool Ask(Action? promptMessageFunc = null)
        {
            while (true)
            {
                if (promptMessageFunc != null)
                    promptMessageFunc();
                var response = Console.ReadLine()!.Trim().ToLowerInvariant();
                if (response is "" or "y" or "yes")
                {
                    return true;
                }
                else if (response is "n")
                {
                    return false;
                }
            }
        }
    }
}
