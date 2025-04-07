# Boss Mood Bar UI - Setup Guide

This guide explains how to set up the Boss Mood Bar UI component in the game.

## Overview

The Boss Mood Bar displays the current mood level of the active boss. As the boss's mood increases, the bar fills up and smoothly transitions colors from green (happy) to yellow (wary) to red (budget-conscious). The bar features an animated gradient that continuously moves to the right, creating a subtle dynamic effect. The bar also displays visual feedback when snaps are added to the deck due to mood changes.

## Prefab Setup

1. Create a new UI GameObject in your hierarchy (or use an existing UI canvas)
2. Add the following elements:

### Main UI Container
- Create a Panel or GameObject named "BossMoodContainer"
- Add the `BossMoodBarUI` script to this GameObject

### Mood Bar Elements
- Create a background bar image (empty border or background)
- Create a fill bar image as a child of the background:
  - Set the Image component's Image Type to "Filled"
  - Set Fill Method to "Horizontal"
  - Set Fill Origin to "Left"
  - Default Fill Amount should be 0 (will be controlled by script)
  - **Important**: Make sure the Image material allows for animated UV offsets (Default UI material works)
- Add a TextMeshProUGUI component near the bar to display the mood status

### Mood Change Feedback
- Create a GameObject for feedback that can be toggled on/off
- Add a background image or panel
- Add a TextMeshProUGUI component to display the feedback message
- This will show messages like "BOSS MOOD ESCALATED!" or "3 SNAPS ADDED TO DECK!"

## Inspector Configuration

Assign the following references in the BossMoodBarUI component:

- **Mood Fill Bar**: Reference to the fill bar Image component
- **Mood Label Text**: Reference to the TextMeshProUGUI showing mood status
- **Mood Change Feedback**: Reference to the feedback GameObject that will be toggled
- **Feedback Text**: Reference to the TextMeshProUGUI for the feedback message

## Color Configuration

You can adjust these in the Inspector:

- **Calm Color**: Default green (0.3, 0.8, 0.3) - Used when boss is happy
- **Wary Color**: Default yellow (0.9, 0.9, 0.2) - Middle state
- **Budget-Conscious Color**: Default red (0.9, 0.3, 0.2) - Most intense state

## Gradient Animation Settings

The bar now features an animated gradient that continuously scrolls to the right:

- **Gradient Anim Speed**: Controls how fast the gradient moves (default: 0.5)
- **Gradient Tiling**: Controls the number of gradient repeats (default: 2)
- **Gradient Contrast**: Controls how strong the gradient effect is (default: 0.2)

## Mood Thresholds

- **Wary Threshold**: Mood value at which the bar starts transitioning to yellow (default: 20)
- **Budget-Conscious Threshold**: Mood value at which the bar starts transitioning to red (default: 40)
- **Max Mood Value**: The maximum mood value for calculating fill amount (default: 60)

## Connecting to Game Flow

In the InitDayEnvironment script, assign the BossMoodBarUI component to the bossMoodBar field.

## Usage Notes

- The bar now uses smooth color transitions between states based on the boss's exact mood level
- The animated gradient continuously scrolls to the right for visual appeal
- The bar automatically updates when:
  - The game state changes
  - The boss mood tier increases
  - Snaps are added to the deck
- The feedback message automatically hides after a delay (configurable)
- The mood status text changes between "HAPPY", "WARY", and "BUDGET-CONSCIOUS" 