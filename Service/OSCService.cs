using System;
using CellularAutomataUI.Helpers;
using SharpOSC;


namespace CellularAutomataUI.Service;


public class OSCService
{
    public static void SendStartMessage() //using it as toggle begin trigger
    {
        var message = new OscMessage("activationState", "Start");
        var sender = new UDPSender("127.0.0.1", 1111);
        sender.Send(message);
    }
    
    public static void SendPauseMessage() //using it as toggle begin trigger
    {
        var message = new OscMessage("activationState", "Pause");
        var sender = new UDPSender("127.0.0.1", 1111);
        sender.Send(message);
    }
    
    
    public static void SendMatrixMessage(string array)
    {
        var message = new OscMessage("/matrix/cellData", array);
        var sender = new UDPSender("127.0.0.1", 2222);
        sender.Send(message);
    }

    public static void SendGridSizeMessage(int size)
    {
        var message = new OscMessage("matrixSize", size);
        var sender = new UDPSender("127.0.0.1", 3333);
        sender.Send(message);
    }

    public static void SendSpeedMessage(int speedSec)
    {
        var message = new OscMessage("iterationSpeed", speedSec);
        var sender = new UDPSender("127.0.0.1", 4444);
        sender.Send(message);
    }

    public static void SendGrainDurationLowMessage(int lowBoundary)
    {
        var message = new OscMessage("grainDurationLow", lowBoundary);
        var sender = new UDPSender("127.0.0.1", 5555);
        sender.Send(message);
    }
    
    public static void SendGrainDurationUpperMessage(int upperBoundary)
    {
        var message = new OscMessage("grainDurationHigh", upperBoundary);
        var sender = new UDPSender("127.0.0.1", 5555);
        sender.Send(message);
    }

    public static void SendReleaseValueForGrainEnvelopMessage(int ReleaseVal)
    {
        var message = new OscMessage("ReleaseValueFromUI", ReleaseVal);
        var sender = new UDPSender("127.0.0.1", 6666);
        sender.Send(message);
    }
}