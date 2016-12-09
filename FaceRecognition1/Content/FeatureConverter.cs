using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luxand;

namespace FaceRecognition1.Content
{
    /// <summary>
    /// Klasa ktora konwertuje wspolrzedne pixelowe cech,
    /// na zaleznosci miedzy cechami podane we flotach.
    /// Jako argument metody przyjmuja tablice FacialFeatures.
    /// </summary>
    public class FeatureConverter
    {
        /// <summary>
        /// Stosunek rozstawu oczu do dlugosci nosa
        /// </summary>
        public static float EyesSpacing_NoseHeigh(FSDK.TPoint[] FacialFeatures)
        {
            int leftEyeX = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_LEFT_EYE].x;
            int leftEyeY = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_LEFT_EYE].y;
            int rightEyeX = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_RIGHT_EYE].x;
            int rightEyeY = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_RIGHT_EYE].y;
           
            int noseTopX = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_NOSE_BRIDGE].x;
            int noseTopY = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_NOSE_BRIDGE].y;
            int noseBottomX = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_NOSE_BOTTOM].x;
            int noseBottomY = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_NOSE_BOTTOM].y;

            double eyeSpace = Math.Sqrt(Math.Pow((double)(rightEyeX - leftEyeX), 2) + Math.Pow((double)(rightEyeY - leftEyeY), 2));
            double noseHeigh = Math.Sqrt(Math.Pow((double)(noseTopX - noseBottomX), 2) + Math.Pow((double)(noseTopY - noseBottomY), 2));

            return (float)(eyeSpace / noseHeigh);
        }

        /// <summary>
        /// Stosunek dlugosci nosa do szerokosci ust
        /// </summary>
        public static float NoseHeigh_MouthWidth(FSDK.TPoint[] FacialFeatures)
        {
            int noseTopX = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_NOSE_BRIDGE].x;
            int noseTopY = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_NOSE_BRIDGE].y;
            int noseBottomX = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_NOSE_BOTTOM].x;
            int noseBottomY = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_NOSE_BOTTOM].y;

            int mouthRightX = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_MOUTH_RIGHT_CORNER].x;
            int mouthRightY = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_MOUTH_RIGHT_CORNER].y;
            int mouthLeftX = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_MOUTH_LEFT_CORNER].x;
            int mouthLeftY = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_MOUTH_LEFT_CORNER].y;

            double mouthWidth = Math.Sqrt(Math.Pow((double)(mouthRightX - mouthLeftX), 2) + Math.Pow((double)(mouthRightY - mouthLeftY), 2));
            double noseHeigh = Math.Sqrt(Math.Pow((double)(noseTopX - noseBottomX), 2) + Math.Pow((double)(noseTopY - noseBottomY), 2));

            return (float)(noseHeigh / mouthWidth);
        }

        /// <summary>
        /// Stosunek rozstawu oczu do szerokosci ust
        /// </summary>
        public static float EyesSpacing_MouthWidth(FSDK.TPoint[] FacialFeatures)
        {
            int leftEyeX = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_LEFT_EYE].x;
            int leftEyeY = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_LEFT_EYE].y;
            int rightEyeX = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_RIGHT_EYE].x;
            int rightEyeY = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_RIGHT_EYE].y;

            int mouthRightX = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_MOUTH_RIGHT_CORNER].x;
            int mouthRightY = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_MOUTH_RIGHT_CORNER].y;
            int mouthLeftX = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_MOUTH_LEFT_CORNER].x;
            int mouthLeftY = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_MOUTH_LEFT_CORNER].y;

            double mouthWidth = Math.Sqrt(Math.Pow((double)(mouthRightX - mouthLeftX), 2) + Math.Pow((double)(mouthRightY - mouthLeftY), 2));
            double eyeSpace = Math.Sqrt(Math.Pow((double)(rightEyeX - leftEyeX), 2) + Math.Pow((double)(rightEyeY - leftEyeY), 2));

            return (float)(eyeSpace / mouthWidth);
        }

        /// <summary>
        /// Stosunek szerokosci twarzy do rozstawu oczu
        /// </summary>
        public static float FaceWidth_EyesSpacing(FSDK.TPoint[] FacialFeatures)
        {
            int faceLeftX = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_FACE_CONTOUR14].x;
            int faceLeftY = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_FACE_CONTOUR14].y;
            int faceRightX = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_FACE_CONTOUR15].x;
            int faceRightY = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_FACE_CONTOUR15].y;

            int leftEyeX = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_LEFT_EYE].x;
            int leftEyeY = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_LEFT_EYE].y;
            int rightEyeX = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_RIGHT_EYE].x;
            int rightEyeY = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_RIGHT_EYE].y;

            double faceWidth = Math.Sqrt(Math.Pow((double)(faceRightX - faceLeftX), 2) + Math.Pow((double)(faceRightY - faceLeftY), 2));
            double eyeSpace = Math.Sqrt(Math.Pow((double)(rightEyeX - leftEyeX), 2) + Math.Pow((double)(rightEyeY - leftEyeY), 2));

            return (float)(faceWidth / eyeSpace);
        }

        /// <summary>
        /// Stosunek dlugosci nosa do szerokosci twarzy
        /// </summary>
        public static float NoseHeigh_FaceWidth(FSDK.TPoint[] FacialFeatures)
        {
            int noseTopX = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_NOSE_BRIDGE].x;
            int noseTopY = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_NOSE_BRIDGE].y;
            int noseBottomX = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_NOSE_BOTTOM].x;
            int noseBottomY = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_NOSE_BOTTOM].y;

            int faceLeftX = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_FACE_CONTOUR14].x;
            int faceLeftY = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_FACE_CONTOUR14].y;
            int faceRightX = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_FACE_CONTOUR15].x;
            int faceRightY = FacialFeatures[(int)FSDK.FacialFeatures.FSDKP_FACE_CONTOUR15].y;

            double faceWidth = Math.Sqrt(Math.Pow((double)(faceRightX - faceLeftX), 2) + Math.Pow((double)(faceRightY - faceLeftY), 2));
            double noseHeigh = Math.Sqrt(Math.Pow((double)(noseTopX - noseBottomX), 2) + Math.Pow((double)(noseTopY - noseBottomY), 2));

            return (float)(noseHeigh / faceWidth);
        }

        //-------------------------------------------------------------------------------

        /// <summary>
        /// Metoda swraca liste wszystkich przygotowanych cech
        /// </summary>
        public static List<float> GetFeatures(FSDK.TPoint[] FacialFeatures)
        {
            List<float> neuralFeatures = new List<float>();

            neuralFeatures.Add(EyesSpacing_NoseHeigh(FacialFeatures));
            neuralFeatures.Add(NoseHeigh_MouthWidth(FacialFeatures));
            neuralFeatures.Add(EyesSpacing_MouthWidth(FacialFeatures));
            neuralFeatures.Add(FaceWidth_EyesSpacing(FacialFeatures));
            neuralFeatures.Add(NoseHeigh_FaceWidth(FacialFeatures));

            return neuralFeatures;
        }
    }
}
