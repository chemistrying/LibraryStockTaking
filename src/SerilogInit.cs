using Serilog;

public class SerilogCreator{
    public SerilogCreator(){
        LoggerConfiguration LogConfig = new LoggerConfiguration().WriteTo.File(Globals._config.DefaultProgramFilesLocation + "libraryLog.log");
        if (Globals._config.LoggingLevel == "Verbose"){
            LogConfig.MinimumLevel.Verbose();
        }else if (Globals._config.LoggingLevel == "Debug"){
            LogConfig.MinimumLevel.Debug();
        }else if (Globals._config.LoggingLevel == "Warning"){
            LogConfig.MinimumLevel.Warning();
        }else if (Globals._config.LoggingLevel == "Error"){
            LogConfig.MinimumLevel.Fatal();
        }else{
            LogConfig.MinimumLevel.Information();
        }

        Log.Logger = LogConfig.CreateLogger();
    }
}