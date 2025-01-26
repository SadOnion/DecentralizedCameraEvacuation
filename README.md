
# Decentralized Camera-Based Evacuation System

## Overview

This project simulates a decentralized camera-based evacuation system where multiple cameras track the number of people entering or exiting an area. These cameras send their updates asynchronously to a central server, which aggregates the data in real-time and provides an accurate count of people on-site.

The system handles edge cases such as:
- Missing or delayed updates.
- Out-of-order data (data arriving with timestamps, but not sequentially).

This solution uses **.NET 8.0+ C#** to simulate camera data ingestion, real-time processing, and people count aggregation.
## Content
1. [Features](#Features)
2. [System Architecture](#SystemArchitecture)
3. [Design Decisions](#DesignDecisions)
4. [Edge Cases Considered](#EdgeCasesConsidered)
5. [Installation](#Installation)
6. [Future Improvements](#FutureImprovements)
7. [Requirements](#Requirements)


## Features

- **Real-time People Count**: Aggregates the number of people on-site based on camera updates.
- **Camera Data Updates**: Each camera sends periodic updates with people entering or exiting.
- **Edge Case Handling**: Handles scenarios like delayed, missing, or out-of-order updates.
- **Scalable Architecture**: The solution is designed to scale with more cameras and real-time data streams.

## Requirements

- .NET 8.0+ SDK

## System Architecture

### Components
1. **Cameras**: Virtual cameras send periodic updates with the number of people entering or exiting, along with timestamps.
2. **CameraMonitoringService**: The central server aggregates camera updates and calculates the real-time people count.
3. **CameraMonitor**: Tracks individual camera information, including the number of people in sight and recent timestamps.
4. **Data Ingestion**: Camera data is ingested asynchronously and processed in real-time.
5. **Real-time Processing**: The central server ensures that data is processed correctly, even if it arrives out-of-order or with delays.

### Design Decisions

- **Data Ingestion**: Each camera sends data asynchronously, simulating the network delays and out-of-order and missing data arrival.
- **Real-Time Processing**: Updates are processed as soon as they arrive, and the system maintains an accurate count of people on-site and per camera.
- **Edge Case Handling**: Missing or delayed updates are handled by checking for time gaps and ensuring the most recent data is used.
- **Out of Order Timestamps**: The OutOfOrderCamera sends data with out-of-sequence timestamps. To handle this, the system always adjusts the data by adding the difference between In and Out, which may cause the data to be temporarily inaccurate or even negative.
- **Missing Data**: Timestamps from cameras are stored in a sorted set and are regularly checked. If any package is missing and hasn't arrived within 10 seconds, the system forces a resynchronization with the camera to retrieve the missing data. This ensures that the system remains accurate and consistent, even if updates are delayed or lost.

Data Processing and People Count Update
The central server updates the total people count with each incoming camera update by calculating In - Out (people entering minus people exiting). This method is simple and fast, ensuring that the system can process large volumes of data quickly. However, this approach can be faulty in cases of unusual or edge scenarios, such as when data arrives out of order or when updates are missing.

Handling Missing or Out-of-Order Data
To address this, the system introduces a missing packet detection mechanism. If a camera's update is delayed or missing and doesn't arrive within 10 seconds, the system triggers a resynchronization with that camera to retrieve the missing data. This prevents the system from processing incomplete or inconsistent data and ensures that the people count remains accurate.

Temporary Inaccuracies Due to Out-of-Order Data
When data from cameras arrives out of order, the In - Out calculation can lead to temporary inaccuracies in the people count. This means that, for a short period, the data may be incorrect or even negative until the system performs necessary adjustments and resynchronization. These inconsistencies are typically brief and are resolved once the data is reordered properly.


### Edge Cases Considered

1. **Camera Downtime**: Cameras may temporarily stop sending data due to network issues.
2. **Delayed Data**: Updates may arrive out of order with older timestamps arriving after newer ones.
3. **Timestamp Errors**: Overlapping or incorrect timestamps are detected and handled.

# Virtual Camera Types: Healthy, Flawed, and OutOfOrder

In the camera-based evacuation system, there are three types of cameras: **Healthy**, **Flawed**, and **OutOfOrder**. Each camera type behaves differently, simulating real-world conditions that can affect data collection.

## 1. Healthy Camera
The **Healthy** camera works perfectly, sending updates regularly with accurate timestamps and people count. It doesn't experience any issues or delays.

## 2. Flawed Camera
The **Flawed** camera occasionally fails to send updates, with a **10% chance** of missing an update. Despite this, the camera still sends accurate data once it's able to send an update.

## 3. OutOfOrder Camera
The **OutOfOrder** camera sends data with out-of-sequence timestamps, simulating network delays or malfunctions. The system checks and adjusts for out-of-order data by validating timestamps.

## Installation

1. Clone this repository to your local machine.
   ```bash
   git clone https://github.com/SadOnion/DecentralizedCameraEvacuation.git
   ```

2. Navigate to the project directory and restore the dependencies:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

4. Run the project:
   ```bash
   dotnet run
   ```

## Usage

Once the application is running, you can query the status of the system using the `/Status` endpoint:

Or use Swagger UI `/swagger`

`default` http://localhost:5043
```http
GET /Status
```

This will return the real-time count of people on-site and, and a breakdown of people count per camera.

Example response:
```json
{
  "TotalPeople": 120,
  "Cameras": {
    "Camera1": 40,
    "Camera2": 35,
    "Camera3": 45
  }
}
```
## Config

The number of cameras can be easily configured in the appsettings.json file. You can modify the counts for each type of camera as follows:
```json
{
  "HealthyCameraCount": 3,
  "OutOfOrderCameraCount": 1,
  "FlawedCameraCount": 1
}
```
By adjusting these values, you can simulate different camera setups and test how the system behaves with varying configurations.
## Code Explanation

1. **`CameraMonitoringService`**: This is the central server that aggregates camera updates. It processes the updates asynchronously and updates the total people count.
2. **`CameraMonitor`**: This class stores information about each camera, including the current people count and the timestamps of the last updates received.
3. **`VirtualCamera`**: Simulates a camera that periodically sends updates to the `CameraMonitoringService` with the number of people entering or exiting.
4. **`CameraUpdate`**: Represents the data structure sent by the cameras, which includes the camera ID, timestamp, and the number of people entering or exiting.


## Future Improvements

- **Persistent Storage**: Implementing a database to store historical data of people counts over time.
- **Advanced Error Handling**: More robust handling for missing or delayed data.
- **Distributed Architecture**: Implementing a distributed system (e.g., using Kafka, Redis, etc.) to handle data streams more effectively.



