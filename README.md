# UiUxGenomeLab

**Research-as-a-Service for UI/UX and beyond.**
Automated overnight research that produces a best-fit, genetically
optimized result.
Search 2.0---starting with a UI/UX proof of concept.

Everyone is welcome to contribute. This project will grow beyond what a
single developer can complete alone.

------------------------------------------------------------------------

## Table of Contents

-   [Overview](#overview)
-   [Concept](#concept)
-   [UIUX POC Direction](#uiux-poc-direction)
-   [High-Level Architecture](#high-level-architecture)
-   [Technical Components](#technical-components)
-   [How It Meets the Goals](#how-it-meets-the-goals)
-   [References](#references)

------------------------------------------------------------------------

## Overview

UiUxGenomeLab introduces **Research-as-a-Service**---the ability to
delegate hours of research and iterative design exploration to an
automated, intelligent system.

Instead of browsing endlessly, the service outputs a **PhD-level
research packet** or a **genetically fittest solution**, as if you had
read every major source and synthesized a complete answer.

This is the next step in the evolution of search:
Not just searching, not just reading---a complete research pipeline that
produces a ranked, explainable outcome.

------------------------------------------------------------------------

## Concept

To realize this, the internet and modern AI had to mature into the
information layer we have today.
Communities like StackExchange also provided essential precedent:
crowdsourced ranking, relevance scoring, and community-validated
solutions.

This system aims to do what designers, engineers, and researchers do
manually:

-   Explore hundreds of variations.
-   Evaluate what performs best.
-   Combine best practices, data points, and expert opinions.
-   Produce a final, confident recommendation.

And do it **while you sleep**.

------------------------------------------------------------------------

## UI/UX POC Direction

The first implementation focuses on **UI/UX design research**.

The system will:

-   Generate every UI pattern combination worth considering
-   Leverage best practices, color theory, accessibility (508/WCAG/AAA)
-   Produce complete templates, components, and layout variations
-   Analyze top-performing UI/UX designs across the web
-   Understand statistical UX performance factors
-   Automatically explore 100s--1000s of iterations

The output includes:

-   Genetically optimized UI/UX candidates
-   Fully generated HTML demo files
-   Research summaries explaining *why* each candidate scored as it did
-   Comparison dashboards and artifacts a designer can review and choose
    from

Designers typically iterate until something "feels right."
This system performs those iterations **programmatically**, comparing
every meaningful combination before you even open the project the next
morning.

------------------------------------------------------------------------

## High-Level Architecture

### API Behavior

You send a POST request:

    POST /jobs/uiux/start

With:

-   **Problem statement**
    e.g., "mobile habit tracker onboarding"
-   **Constraints**
    e.g., brand colors, platform, design system rules
-   **Research duration**
    e.g., 8 hours, or fixed generation count

The service then creates an **asynchronous background research job**.

### Research Job Loop

Each job:

1.  Generates a population of UI/UX design specs using the OpenAI
    Responses API

2.  Refines prompts and search queries using

    -   OpenAI `web_search`
    -   optional external search providers (Google, Bing, etc.)

3.  Runs a genetic algorithm:

        generate → score → select → mutate → repeat

4.  For every candidate:

    -   Writes a standalone HTML demo
    -   Stores style tokens (palette, type, layout, components)
    -   Tracks scores, ranking data, and comparison notes

5.  At the end:

    -   Writes a complete JSON + Markdown/HTML **research bundle**
    -   Writes an **index.html** with links to every candidate and
        comparison tables

------------------------------------------------------------------------

## Technical Components

-   **ASP.NET Core Minimal API** (.NET 10)
-   **BackgroundService** for long-running job orchestration
-   **OpenAIResponseClient** for:
    -   structured UI/UX design JSON
    -   research assistance via `web_search`
-   **Optional ISearchProvider abstraction** for Google/Bing API usage
-   **Configurable file output location**
-   **Batch-oriented and cost-aware Responses usage**

------------------------------------------------------------------------

## How It Meets the Goals

**Long-running execution**
- Uses `MaxGenerations` + `MaxDuration` and a background service to run
overnight or longer.

**Latest Microsoft & OpenAI tooling**
- Built on `.NET 10.0` and the official OpenAI .NET SDK (NuGet).

**Genetic search optimization**
- Population generation, scoring, elite selection, mutation cycles.

**Cost-aware execution**
- Populations generated via a single Responses call.
- Batch scoring per generation.

**Smart search & query refinement**
- PromptRefinementService
- ISearchProvider
- OpenAI Responses `web_search`

**Output artifacts**
- `research-bundle.json`
- `index.html`
- One HTML demo file per candidate

**Style inspection**
- Each candidate exposes palette, typography, layout notes, navigation
structure, and design tokens.

------------------------------------------------------------------------

## References

-   https://openai.com/
-   https://github.com/openai/openai-dotnet
-   https://github.com/openai/openai-dotnet?tab=readme-ov-file#how-to-use-responses-with-web-search
-   https://www.nuget.org/packages/OpenAI
-   https://chatgpt.com/
-   https://github.com/openai/openai-dotnet/blob/main/docs/Observability.md
-   https://www.postman.com/
-   https://copilot.microsoft.com/
-   https://support.google.com/websearch/thread/135474043/how-do-i-get-web-search-results-using-an-api
-   https://developers.google.com/custom-search/v1/overview
