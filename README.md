iRacingDataLogger

This program gets telemetry data from the iRacing SDK and writes it out to IRPerfData.csv (in the same directory as the binary was run).

The goal is to collect performance data about Framerate, CPU and GPU utilization for profiling settings and hardware.

When used with an iRacing replay file, it can provide a way to visualize the performance of the same simulation events with different configurations.

The output file has a header row, and then data for as long as its run.  

The same IRPerfData.csv file is used and overwritten every time its executed, so rename/move after each run if you want to save the data.

A sample of the output looks like so:

ReplaySessionNum,ReplaySessionTime,ReplayFrameNum,FrameRate,CpuUsageBG,CpuUsageFG,GpuUsage
2,99,41266,89.38937377929688,24.69994354248047,59.8281135559082,61.23029708862305
2,99,41266,90.21427154541016,25.799945831298828,63.55421447753906,66.07955932617188
2,99,41266,89.12008666992188,25.599946975708008,64.16100311279297,64.92153930664062
2,99,41266,87.92404174804688,25.09994888305664,62.76055145263672,65.34321594238281
2,99,41266,88.92002868652344,24.799943923950195,61.33407974243164,62.35883331298828
...

Please remain calm.
