# HwInfoLogAnalayzer

A log analyzer for HWiNFO64 csv format log output. 

## Motivation

I want to know the built-in VF curve for each CPU core, so that I can tune each core to its max potential.

## Usage
1. Open log file.
2. Click 'Start', it will start monitoring the log file change, periodically accumulating newly appended VF points.
3. Click legend entry on the right to toggle each core's visibility. Hover VF point to see the voltage and frequency value.

## Notes
* At least for AMD CPU, enable/disable the 'Snapshot CPU polling' setting in HWiNFO64 main settings yields vastly different result. 
* At least for AMD CPU, one specific voltage can point to multiple clock frequencies, which is very puzzling.
