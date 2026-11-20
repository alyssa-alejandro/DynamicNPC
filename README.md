# Adaptive Companion AI — Real-Time Player-Driven NPC

A Unity AI architecture project featuring a real-time **Utility AI companion** that continuously observes player playstyle telemetry, filters noisy inputs, and adapts NPC behaviors and NavMesh movement dynamically.

---

## Overview

Traditional scripted companions often feel static or overly reactive to momentary player inputs. **Adaptive Companion AI** solves this by pairing an **Exponential Moving Average (EMA) telemetry filter** with a **dynamic Utility AI scoring pipeline** in Unity (C#). The companion tracks playstyle metrics over time, evaluates candidate actions on throttled intervals, and uses inertia biasing to commit smoothly to context-appropriate behaviors.

---

## Key Features

- **Real-Time Utility AI Pipeline:** Continuously evaluates player playstyle telemetry and dynamically scores candidate NPC actions to select the most contextually appropriate behavior.
- **EMA Telemetry Filtering:** Applies an Exponential Moving Average (EMA) filter with telemetry decay to smooth raw gameplay metrics into stable playstyle signals.
- **Decision Stability & Anti-Flicker:** Combines **inertia biasing** with **0.2s evaluation throttling** to eliminate erratic state flickering caused by noisy telemetry inputs.
- **NavMesh Navigation:** Integrates Unity's NavMesh system for responsive spatial positioning and companion movement tied to active utility states.
- **Custom Unity Editor Debug UI:** Built-in Editor tooling that visualizes real-time utility scores, telemetry decay curves, and active decision outputs for rapid parameter tuning and iteration.

---

## System Architecture

```text
[Player Gameplay Telemetry]
            │
            ▼
  ┌───────────────────┐
  │    EMA Filter     │  ◄── Smooths raw inputs & applies telemetry decay
  └─────────┬─────────┘
            │
            ▼
  ┌───────────────────┐
  │  Utility Scorer   │  ◄── Evaluates actions every 0.2s + applies inertia bias
  └─────────┬─────────┘
            │
            ▼
  ┌───────────────────┐
  │  NPC Execution    │  ◄── Executes winning behavior & NavMesh navigation
  └─────────┬─────────┘
            │
            ▼
  ┌───────────────────┐
  │  Editor Debug UI  │  ◄── Live inspection of scores, decay & state transitions
  └───────────────────┘