# EthMonitoring

Mining monitoring tool with Android and iOS application support. Also supports Telegram, SMS and Push notifications with custom sounds.

# For linux version check this:

https://github.com/JamesSmith2/EthMonitoringLinux

# Current version

- Support SMS Sending when:

	-- GPU temperature gets higher than required

	-- Miner is not responding for X amount of minutes

	-- Hashrate below X


- Supports multiple miners with one local software installed.
- Supports EWBF API
- Supports Claymore's Dual Ethereum GPU Miner 9.3-9.8
- Supports CCMiner-Alexis 1.0
- Supports SGMiner
- Supports Excavator by NiceHash

# Web service

- Supports profit monitoring
- Mobile friendly
- Profit graph
- Gpu graph (coming soon)
- Telegram support
- SMS Support

# Requirements

- This tool required .NET Framework 4.6.1 installed (https://www.microsoft.com/en-us/download/details.aspx?id=49981)

# How to use

This release supports only windows systems. 

Multiple miners supported, only needed to install one of the local miners. 

Access token can be received and monitored from here: https://ethmonitoring.com

# Guide

- Full guide available here: https://ethmonitoring.com/site/guide

# Steps

- Install windows application only for one local miner.

- Add local hosts example: 192.168.1.1 and name: MyMiner1 (For custom port use: 192.168.1.1:3333)

- Register on site to get the token

- Insert token to box and press "Start monitoring"

- Data will be uploaded to site and notifications can be added from there.

# How to update

When downloading new files, extract them over or copy settings.json file to new folder where new binary is.

# FAQ

- Claymore's miner v9.4 needs to be enabled in firewall, v9.3 doesn't have this problem.
- EWBF needs to have --api 0.0.0.0:42000 command line parameter added to enable API access.
- CCMiner-Alexis 1.0 needs to have --api-bind=0.0.0.0:4068 parameter in the command line for API access.

# Supports

- Claymore's Dual Ethereum + Decred/Siacoin/Lbry/Pascal AMD+NVIDIA GPU Miner. https://bitcointalk.org/index.php?topic=1433925.0
- EWBF's CUDA Zcash miner https://bitcointalk.org/index.php?topic=1707546.0
- CCMiner-Alexis 1.0

# Developer

This software is free to use for everbody, but you can always support the developer: 

BTC: 1JUrgoekaQm7H4ARrvduGGCmWkrzUoxKBk

ETH: 0xD9B9200eE4017A0E07089629475DbaBA611Fc4e6

ZEC: t1VUN4kfNiZQmwZ7dL3X6XujrCU3A6VMfbh
