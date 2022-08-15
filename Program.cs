using Spectre.Console;
using System.Diagnostics;

namespace AssaultCubeExternal
{
    internal class Program
    {
        internal static List<string> EnabledHacks = new List<string>();

        static void Main()
        {
            Console.Title = "Assault Cube External | Made by Kye#5000";
            AnsiConsole.MarkupLine("[red]Finding ac_client.exe ...[/] ");
            bool ErrorMsg = false;
        start: Process[] processList = Process.GetProcessesByName("ac_client");
            if (processList.Length > 0)
            {
                Memory.SetProcess(processList[0]);
            }
            else
            {
                if (ErrorMsg == false)
                {
                    AnsiConsole.MarkupLine("[red]Assault Cube is not open! Retrying...[/] ");
                    ErrorMsg = true;
                }


                Thread.Sleep(1500);
                goto start;
            }

            AnsiConsole.MarkupLine($"[green]Found Assault Cube! Process ID: {Memory.process.Id}[/]");




            List<string> HackList = new List<string>() { "No Recoil", "Unlimited Health", "Unlimited Armor", "Unlimited Ammo" };
            Task.Run(() => Hack.HackThreadAsync());
            while (true)
            {
                Console.Clear();
                AnsiConsole.Write(new FigletText("Assault Cube Cheat") { Color = Color.Aqua });

                var prompt = new MultiSelectionPrompt<string>();
                prompt.Title = "[red]Hack List:[/]";
                prompt.NotRequired();
                prompt.AddChoices(HackList);
                prompt.PageSize(100);

                foreach (string item in EnabledHacks)
                {
                    prompt.Select(item);
                }
                prompt.InstructionsText("[grey]Press [blue]<space>[/] to toggle a hack, [green]<enter>[/] to accept)[/]");
                EnabledHacks = AnsiConsole.Prompt(prompt);
            }
        }
    }
}