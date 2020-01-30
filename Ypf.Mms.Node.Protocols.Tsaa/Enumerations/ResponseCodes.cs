
namespace Ypf.Mms.Node.Protocols.Tsaa.Enumerations
{
    public enum ResponseCodes : byte
    {
        RequestWasSuccessful = 0,
        NoBufferAvailable = 1,
        BinNumberWasNotInRange = 2,
        CommunicationModuleIsBusy = 3,
        NoMpIsRunning = 4,
        TSXHasRejectedTheRequest = 5,
        RequestToTSXTimedOut = 6,
        InvalidResponseFromTX = 7,
        MessageWasTooBig = 8,
        OffsetOrNumberOfValuesWasInvalid  = 9,
        NoControlProgram = 10,
        ReadOnlyPort =11,
        BadSOENumber = 236,
        InvalidSOEType = 237,
        InvalidSOEState = 238
    }
}
