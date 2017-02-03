using System;
using Tangible.Shared;
using UnityEngine;

namespace Tangible.WordsBattleship {
    public enum ApplicationState {
        MainMenu,
        CharacterSelect,
        WordSelect,
    }

    [CreateAssetMenu(fileName="ApplicationConstants", menuName="ApplicationConstants")]
    public class ApplicationConstants : ScriptableObject {
        public static ApplicationConstants Instance;

        public static readonly string[] kRandomWords = new string[] { "without", "before", "large", "million", "must", "home", "under", "water", "room", "write", "mother", "area", "national", "money", "story", "young", "fact", "month", "different", "lot", "study", "book", "eye", "job", "word", "though", "business", "issue", "side", "kind", "four", "head", "far", "black", "long", "both", "little", "house", "yes", "since", "provide", "service", "around", "friend", "important", "father", "sit", "away", "until", "power", "hour", "game", "often", "yet", "line", "political", "end", "among", "ever", "stand", "bad", "lose", "however", "member", "pay", "law", "meet", "car", "city", "almost", "include", "continue", "set", "later", "community", "name", "five", "once", "white", "least", "president", "learn", "real", "change", "team", "minute", "best", "several", "idea", "kid", "body", "information", "nothing", "ago", "lead", "social", "understand", "whether", "watch", "together", "follow", "parent", "stop", "face", "anything", "create", "public", "already", "speak", "others", "read", "level", "allow", "add", "office", "spend", "door", "health", "person", "art", "sure", "war", "history", "party", "within", "grow", "result", "open", "morning", "walk", "reason", "low", "win", "research", "girl", "guy", "early", "food", "moment", "himself", "air", "teacher", "force", "offer", "enough", "education", "across", "although", "remember", "foot", "second", "boy", "maybe", "toward", "able", "age", "policy", "everything", "love", "process", "music", "including", "consider", "appear", "actually", "buy", "probably", "human", "wait", "serve", "market", "die", "send", "expect", "sense", "build", "stay", "fall", "oh", "nation", "plan", "cut", "college", "interest", "death", "course", "someone", "experience", "behind", "reach", "local", "kill", "six", "remain", "effect", "yeah", "suggest", "class", "control", "raise", "care", "perhaps", "late", "hard", "field", "else", "pass", "former", "sell", "major", "sometimes", "require", "along", "development", "themselves", "report", "role", "better", "economic", "effort", "decide", "rate", "strong", "possible", "heart", "drug", "leader", "light", "voice", "wife", "whole", "police", "mind", "finally", "pull", "return", "free", "military", "price", "less", "according", "decision", "explain", "son", "hope", "develop", "view", "relationship", "carry", "town", "road", "drive", "arm", "TRUE", "federal", "break", "difference", "thank", "receive", "value", "international", "building", "action", "full", "model", "join", "season", "society", "tax", "director", "position", "player", "agree", "especially", "record", "pick", "wear", "paper", "special", "space", "ground", "form", "support", "event", "official", "whose", "matter", "everyone", "center", "couple", "site", "project", "hit", "base", "activity", "star", "table", "court", "produce", "eat", "teach", "oil", "half", "situation", "easy", "cost", "industry", "figure", "street", "image", "itself", "phone", "either", "data", "cover", "quite", "picture", "clear", "practice", "piece", "land", "recent", "describe", "product", "doctor", "wall", "patient", "worker", "news", "test", "movie", "certain", "north", "personal", "simply", "third", "technology", "catch", "step", "baby", "computer", "type", "attention", "draw", "film", "Republican", "tree", "source", "red", "nearly", "organization", "choose", "cause", "hair", "century", "evidence", "window", "difficult", "listen", "soon", "culture", "billion", "chance", "brother", "energy", "period", "summer", "realize", "hundred", "available", "plant", "likely", "opportunity", "term", "short", "letter", "condition", "choice", "single", "rule", "daughter", "administration", "south", "husband", "Congress", "floor", "campaign", "material", "population", "economy", "medical", "hospital", "church", "close", "thousand", "risk", "current", "fire", "future", "wrong", "involve", "defense", "anyone", "increase", "security", "bank", "myself", "certainly", "west", "sport", "board", "seek", "per", "subject", "officer", "private", "rest", "behavior", "deal", "performance", "fight", "throw", "top", "quickly", "past", "goal", "bed", "order", "author", "fill", "represent", "focus", "foreign", "drop", "blood", "upon", "agency", "push", "nature", "color", "recently", "store", "reduce", "sound", "note", "fine", "near", "movement", "page", "enter", "share", "common", "poor", "natural", "race", "concern", "series", "significant", "similar", "hot", "language", "usually", "response", "dead", "rise", "animal", "factor", "decade", "article", "shoot", "east", "save", "seven", "artist", "scene", "stock", "career", "despite", "central", "eight", "thus", "treatment", "beyond", "happy", "exactly", "protect", "approach", "lie", "size", "dog", "fund", "serious", "occur", "media", "ready", "sign", "thought", "list", "individual", "simple", "quality", "pressure", "accept", "answer", "resource", "identify", "left", "meeting", "determine", "prepare", "disease", "whatever", "success", "argue", "cup", "particularly", "amount", "ability", "staff", "recognize", "indicate", "character", "growth", "loss", "degree", "wonder", "attack", "herself", "region", "television", "box", "TV", "training", "pretty", "trade", "election", "everybody", "physical", "lay", "general", "feeling", "standard", "bill", "message", "fail", "outside", "arrive", "analysis", "benefit", "sex", "forward", "lawyer", "present", "section", "environmental", "glass", "skill", "sister", "PM", "professor", "operation", "financial", "crime", "stage", "ok", "compare", "authority", "miss", "design", "sort", "act", "ten", "knowledge", "gun", "station", "blue", "strategy", "clearly", "discuss", "indeed", "truth", "song", "example", "democratic", "check", "environment", "leg", "dark", "various", "rather", "laugh", "guess", "executive", "prove", "hang", "entire", "rock", "forget", "claim", "remove", "manager", "enjoy", "network", "legal", "religious", "cold", "final", "main", "science", "green", "memory", "card", "above", "seat", "cell", "establish", "nice", "trial", "expert", "spring", "firm", "Democrat", "radio", "visit", "management", "avoid", "imagine", "tonight", "huge", "ball", "finish", "yourself", "theory", "impact", "respond", "statement", "maintain", "charge", "popular", "traditional", "onto", "reveal", "direction", "weapon", "employee", "cultural", "contain", "peace", "pain", "apply", "measure", "wide", "shake", "fly", "interview", "manage", "chair", "fish", "particular", "camera", "structure", "politics", "perform", "bit", "weight", "suddenly", "discover", "candidate", "production", "treat", "trip", "evening", "affect", "inside", "conference", "unit", "style", "adult", "worry", "range", "mention", "deep", "edge", "specific", "writer", "trouble", "necessary", "throughout", "challenge", "fear", "shoulder", "institution", "middle", "sea", "dream", "bar", "beautiful", "property", "instead", "improve", "stuff" };

        public static readonly char[] kLetters = new char[] {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        };


        // PRAGMA MARK - Public Interface
        [Header("Debug")]
        public ApplicationState InitialState = ApplicationState.MainMenu;
        public string InitialStateName {
            get {
                switch (InitialState) {
                    case ApplicationState.MainMenu:
                        return "Main Menu";
                    case ApplicationState.CharacterSelect:
                        return "Character Select";
                    case ApplicationState.WordSelect:
                        return "Word Select";
                    default:
                        return "Default";
                }
            }
        }

        [Header("Properties")]
        public Character[] AllCharacters;

        [Space]
        public int MaxWordLength = 10;
    }
}
