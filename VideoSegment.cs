using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tgif_clarifi
{
    /**
 * Provides recognition results for a short interval of a video.
 * <p>
 * When the input to a {@link RecognitionRequest} is video content, the corresponding
 * {@link RecognitionResult} will contain a list of VideoSegments, each of which may contain tags
 * and/or an embedding vector for a short interval of the video.
 */
    public class VideoSegment
    {
        private readonly double timestamp;
        private readonly List<Tag> tags;
        private readonly double[] embedding;

        VideoSegment(double timestamp, List<Tag> tags, double[] embedding)
        {
            this.timestamp = timestamp;
            this.tags = tags;
            this.embedding = embedding;
        }

        /** Returns timestamp of the start of this segment, in seconds since the start of the video. */
        public double getTimestampSeconds()
        {
            return timestamp;
        }

        /**
         * Returns a list of {@link Tag}s for the segment. This will be null if tags were not requested.
         */
        public List<Tag> getTags()
        {
            return tags;
        }

        /**
         * Returns an embedding vector representing the segment. This will be null if embeddings were
         * not requested.
         */
        public double[] getEmbedding()
        {
            return embedding;
        }
    }
}
