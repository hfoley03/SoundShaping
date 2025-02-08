# SoundShaping
## A Creative Art Therapy Activity in Augmented Reality for Neurodevelopmental Disorders
### Thesis Project Music & Acoustic Engineering
#### Conducted at i3Lab, Politecnic di Milano
#### Grade Awarded: 7/7

Have a read:
- [Thesis](https://drive.google.com/file/d/1nowwdGHTUXjbxAIOSJBoOaD5fLPqJFBH/view?usp=sharing)
- [Executive Summary](https://drive.google.com/file/d/1YiPZeVluO9wOEzyRvv8u9xBRbw5y9d_0/view?usp=sharing)
- CHI Paper on request

## Overview
SoundShaping is an augmented reality (AR) application designed to promote creative expression and art therapy, specifically for individuals with neurodevelopmental disorders such as Autism Spectrum Disorder (ASD). The system leverages the Microsoft Hololens 2 and incorporates interactive drawing, sculpting, and musical composition to provide a multi-sensory, immersive experience.

This project combines 3D drawing mechanics and music co-creation, allowing users to draw in virtual space, interact with their drawings, and generate harmonious music without the need for musical training.

![alt text](https://github.com/hfoley03/DrawPlaySculpt/blob/main/Images/oe_tmt.png?raw=true)

## Features
- **3D Drawing and Sculpting:** Users can draw 3D lines that resemble pipes or tubes, which are converted into **cubic Bézier curves** for further manipulation and sculpting.
- **Music Co-Creation:** A **stochastic probabilistic model with variable-length Markov chains** controls the chord progressions, allowing users to interact with musical notes that follow the drawings.
- **Modes of Play:**
  - **Open-Ended Mode:** Encourages free-form interaction and creativity, allowing users to explore artistic expression.
  - **Structured Mode:** Inspired by the **Trail Making Test**, where users connect numbered nodes in a specific order to complete a task.
- **TouchOSC Integration:** The system uses TouchOSC for the practitioner or researcher to control the color of the lines and other parameters via a tablet interface.
- **Interaction Management:** An advanced Interaction Manager handles hand-based gestures for drawing, sculpting, and interacting with note nodes using state machines for both hands.
- **Therapeutic Focus:** Designed with inclusivity in mind, the system is intended to cater to individuals with different intellectual and physical abilities, promoting self-expression, emotional regulation, and cognitive development.

## Research Questions

1. Could a drawing and music therapy activity in augmented reality help to **promote emotional regulation** in adults with moderate neurodevelopmental disorders with or without intellectual disability?
2. Could a drawing and music therapy activity in augmented reality **positively impact selective and sustained attention** in adults with moderate neurodevelopmental disorders with or without intellectual disability?
3. Could a drawing and music therapy activity in augmented reality **create engagement through performance** in adults with moderate neurodevelopmental disorders with or without intellectual disability?


## Installation

1. **Clone the repository**
2. **Set up Unity:**
Install Unity 2021.3.x or later with the Universal Render Pipeline (URP) and MRTK 3.0.
3. **Open the Project:**
Open the project folder in Unity.
4. **Deploy to Hololens 2:**
Follow standard Unity deployment steps for the Hololens 2. Make sure you have the appropriate development environment set up, including Visual Studio and Windows SDK.
