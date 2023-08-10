import { AskRequest, AskResponse, ChatRequest } from "./models";
// import dotenv from "dotenv";

// dotenv.config();

export async function askApi(options: AskRequest): Promise<AskResponse> {
    const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/api/ask`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "x-functions-key": import.meta.env.VITE_API_KEY
        },
        body: JSON.stringify({
            question: options.question,
            approach: options.approach,
            overrides: {
                semantic_ranker: options.overrides?.semanticRanker,
                semantic_captions: options.overrides?.semanticCaptions,
                top: options.overrides?.top,
                temperature: options.overrides?.temperature,
                prompt_template: options.overrides?.promptTemplate,
                prompt_template_prefix: options.overrides?.promptTemplatePrefix,
                prompt_template_suffix: options.overrides?.promptTemplateSuffix,
                exclude_category: options.overrides?.excludeCategory
            }
        })
    });

    console.log("RHYMOND =>", response);
    
    const parsedResponse: AskResponse = await response.json();
    console.log("RHYMOND =>", parsedResponse);

    if (response.status > 299 || !response.ok) {
        throw Error(parsedResponse.Error || "Unknown error");
    }

    return parsedResponse;
}

export async function chatApi(options: ChatRequest): Promise<AskResponse> {
    console.log("RHYMOND", import.meta.env.VITE_API_BASE_URL, import.meta.env.VITE_API_KEY);

    const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/api/chat`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "x-functions-key": import.meta.env.VITE_API_KEY
        },
        body: JSON.stringify({
            history: options.history,
            approach: options.approach,
            overrides: {
                semantic_ranker: options.overrides?.semanticRanker,
                semantic_captions: options.overrides?.semanticCaptions,
                top: options.overrides?.top,
                temperature: options.overrides?.temperature,
                prompt_template: options.overrides?.promptTemplate,
                prompt_template_prefix: options.overrides?.promptTemplatePrefix,
                prompt_template_suffix: options.overrides?.promptTemplateSuffix,
                exclude_category: options.overrides?.excludeCategory,
                suggest_followup_questions: options.overrides?.suggestFollowupQuestions
            }
        })
    });

    console.log("RHYMOND:", response);

    const parsedResponse: AskResponse = await response.json();
    console.log("RHYMOND:", parsedResponse);

    if (response.status > 299 || !response.ok) {
        throw Error(parsedResponse.Error || "Unknown error");
    }

    return parsedResponse;
}

export function getCitationFilePath(citation: string): string {
    return `${import.meta.env.VITE_API_BASE_URL}/api/content/${citation}`;
}
