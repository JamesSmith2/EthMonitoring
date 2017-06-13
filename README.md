# EthMonitoring
Claymore's Dual Ethereum GPU Miner monitoring with SMS Support

# Current version

- Support SMS Sending when:

	-- GPU temperature gets higher than required

	-- Miner is not responding for X amount of minutes

	-- Hashrate below X


- Supports multiple miners with one local software installed.
- Supports EWBF API
- Supports Claymore's Dual Ethereum GPU Miner 9.3-9.4
- Supports CCMiner-Alexis 1.0

# Web service

- Supports profit monitoring
- Mobile friendly
- Profit graph
- Gpu graph (coming soon)
- Telegram support
- SMS Support

# How to use

This release supports only windows systems. 

Multiple miners supported, only needed to install one of the local miners. 

Access token can be received and monitored from here: http://monitoring.mylifegadgets.com

# Steps

- Install windows application only for one local miner.

- Add local hosts example: 192.168.1.1 and name: MyMiner1 (For custom port use: 192.168.1.1:3333)

- Register on site to get the token

- Insert token to box and press "Start monitoring"

- Data will be uploaded to site and notifications can be added from there.

- SMS Services are used by https://textbelt.com API key can be collected from that site and inserted in settings section on the web.


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

BTC: 17x5FEZ4dT8QyXEE9ou73GT9ZKppfrNify

ETH: 0xbfb5e20e58cb7fa67fbbf3f193ba349d71b276ef

ZEC: t1VUN4kfNiZQmwZ7dL3X6XujrCU3A6VMfbh
