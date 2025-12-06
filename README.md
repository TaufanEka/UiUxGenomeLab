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
-   [How It Meets the Goals 508 WCAG ADA](#how-it-meets-the-goals-508-wcag-ada)
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

## How It Meets the Goals 508 WCAG ADA
- Theory How AI will be used to automatically translate ANY website into a 508 WCAG Compliance A-AAA
- How AI morphs to meet any user preference(s)0 of ADA requirement, then eventually expand beyond just compliance to meet even more ADA need. 
- Overall, supporting as many users as possible with the same full user experience, using AI behind the scenes to translate any website into a fully compliant output. AKA deliver a translation per user, per AI translating to meet user set preference(s).
- Meeting the goals 508 WCAG ADA overall compliance using AI to morph the website into a full delivery, same as non ADA, through adapting to any user preference.
- The logic of meeting full 508 compliance.
- The logic of meeting full WCAG to AAA ratios.
- The logic of supporting the largest ADA audience possible, aka as many users as possible, first focusing on the law. Then research expanding to meet further ADA need.
- Unit tests generated to meet the highest percentage chance as the backbone requirements to meet full 508 + WCAG compliance.
- Translate unit tests into logic for the AI to consider as mandatory requirements to consider, then solving also for the remainder of generating UI UX that meets full compliance. The whole puzzle, not just some pieces aka.
- Then build into a sub service using AI as a service to live translate any website into a full 508 + WCAG compliant output as one of two ways, or both.
Way 1) Would be a plug-in that any website can install that would allow any user to set their preferences to toggle each of their ADA disabilities, then the AI morphing the website to enable their full use the same as a non ADA user would be able to use the same site, except that the AI plug-in would morph the website to meet any preference toggled on, for all supportable ADA preferences that the user toggled on in their preferences.
Way 2) A 508 tool, app, or computer program for all of ADA to use, which uses the same approach to AI, except the tool would live translate (morph) any website into the compliant fully complete delivery, for any website that the user visits, as they visit. So 508 compliance and WCAG requirements would no longer need to be met so long as ADA instead simply installed this tool, program, or app, which would ensure that ADA always was garanteed full functionality of every website, no matter if the website followed any 508 compliance, there would no longer be a need nor reason for a website to struggle to meet all audience need and design the inbetween, any longer, if we can use this way as a tool for ADA to auto handle live translation for any 508 or WCAG law first, then meet more ADA audience need to further the research onwards. This would be really nice and I would like to request help, aside from this project, just to create this POC and I will offer to share selling it to 508 ADA as a number one most downloaded most nescessary tool to SOLVE FULLY finally, for the whole user experience. I really like this idea, we should create a GitHub aside from this project just for this, and begin preparing a proof of concept which we can present to ADA. Even the AI that can live morph any website into meeting 508 ADA WCAG, itself would be a major MAJOR long term problem solved for ADA as a whole. AI is the key, hoping that every website in the world will meet compliance is the hardest approach, which does not support users well. This tool would be the complete solution finally, AI is the proven middle service, it should be trained as POC to demo that it can translate ANY and all websites into a 508 WCAG delivery, for all legally supported disabilities, to prove the sale to ADA, at that point the whole world can download, install, or be translated on the spot if Way 1, was used which the website itself would have installed the plug-in. This proves why a 508 app or tool or software program is the 100% winner to AI live translation to ensure 100% user experience delivery.

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
-   https://www.section508.gov/
-   https://www.w3.org/
-   https://www.wcag.com/
-   https://www.ada.gov/
-   https://www.section508.gov/develop/applicability-conformance
-   https://www.w3.org/WAI/standards-guidelines/wcag/
-   https://www.ada.gov/resources/web-guidance/
