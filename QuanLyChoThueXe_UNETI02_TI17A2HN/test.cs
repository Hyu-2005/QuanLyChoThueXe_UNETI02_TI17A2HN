using System;
public class Test {
    public static void Main() {
        DateTime? dt = null;
        Console.WriteLine("Val: " + dt?.ToString("yyyy"));
    }
}
