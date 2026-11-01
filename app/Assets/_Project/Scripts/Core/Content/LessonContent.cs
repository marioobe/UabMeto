using System;

namespace UabMeto.Core.Content
{
    /// <summary>
    /// DTO konten yang dipakai UI. Field mengikuti skema 1.1.0.
    /// Kelas ini tidak melakukan validasi; gate ada di PublishGate dan
    /// ContentRepository.
    /// </summary>
    [Serializable]
    public sealed class TopicRegistry
    {
        public string schemaVersion;
        public string variant;
        public string source;
        public string notes;
        public TopicEntry[] topics;
    }

    [Serializable]
    public sealed class TopicEntry
    {
        public string id;
        public int order;
        public string title;
        public string icon;
        public string status;
        public string[] lessons;
    }

    [Serializable]
    public sealed class LessonContent
    {
        public string schemaVersion;
        public string id;
        public string topicId;
        public int order;
        public int xp;
        public int estimatedMinutes;
        public string title;
        public string summary;
        public string status;
        public LessonValidationData validation;
        public string[] objectives;
        public VocabularyItem[] vocabulary;
        public Dialogue[] dialogues;
        public Exercise[] exercises;
        public ExampleItem[] examples;
        public string notes;
    }

    [Serializable]
    public sealed class LessonValidationData
    {
        public string reviewedBy;
        public string reviewedAt;
        public SourceRef[] sources;
        public string audioLicense;
        public string consentRef;
    }

    [Serializable]
    public sealed class SourceRef
    {
        public string id;
        public string citation;
        public string url;
        public string license;
    }

    [Serializable]
    public sealed class AudioRef
    {
        public string file;
        public int durationMs;
        public int bitrateKbps;
        public string license;
        public string consentRef;
    }

    [Serializable]
    public sealed class VocabularyItem
    {
        public string id;
        public string dawan;
        public string indonesian;
        public string notes;
        public AudioRef audio;
    }

    [Serializable]
    public sealed class Dialogue
    {
        public string id;
        public string title;
        public DialogueLine[] lines;
    }

    [Serializable]
    public sealed class DialogueLine
    {
        public string speaker;
        public string dawan;
        public string indonesian;
        public AudioRef audio;
    }

    [Serializable]
    public sealed class Exercise
    {
        public string id;
        public string type;
        public string skill;
        public string prompt;
        public string explanation;
        public ExerciseContent content;
    }

    [Serializable]
    public sealed class ExerciseContent
    {
        public ExerciseOption[] options;
        public string correctOptionId;
        public string audioRef;
        public string targetAudioRef;
        public string expectedAnswer;
        public string[] tokens;
        public string[] correctOrder;
    }

    [Serializable]
    public sealed class ExerciseOption
    {
        public string id;
        public string text;
    }

    [Serializable]
    public sealed class ExampleItem
    {
        public string id;
        public string dawan;
        public string indonesian;
        public string context;
        public string status;
        public string sourceRef;
        public AudioRef audio;
    }
}
