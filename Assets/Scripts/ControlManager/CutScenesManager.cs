using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CutScenesManager : MonoBehaviour
{
        [SerializeField] private PlayableDirector finisherSuccess;
        [SerializeField] private PlayableDirector finisherFail;

        [SerializeField] private string _enemyTrackName = "EnemyTrack";
        
        private Action _currentOnCompleteCallback;

        private void OnEnable()
        {
                finisherFail.stopped += OnDirectorStopped;
                finisherSuccess.stopped += OnDirectorStopped;

        }

        private void OnDisable()
        {
                finisherFail.stopped -= OnDirectorStopped;
                finisherSuccess.stopped -= OnDirectorStopped;
        }
        /// <summary>
        /// Gọi hàm này khi bắt đầu Finisher
        /// </summary>
        /// <param name="enemyInstance">Con quái thực tế đang đứng trong game</param>
        public void PlayCinematic(PlayableDirector _director,Actor enemyInstance, Action onCompleted = null)
        {
                
                _currentOnCompleteCallback = onCompleted;
                // 1. Lấy Timeline Asset
                var timelineAsset = (TimelineAsset)_director.playableAsset;

// 2. Lấy Animator cần gán
                var enemyAnimator = enemyInstance.GetComponent<Animator>();

// 3. LỌC TRACK: Dùng .Where() để lấy tất cả track có tên trùng khớp
                var targetTracks = timelineAsset.GetOutputTracks()
                        .Where(t => t.name == _enemyTrackName);

// Kiểm tra xem có tìm thấy track nào không (Optional)
                if (!targetTracks.Any())
                {
                        Debug.LogError($"Không tìm thấy track nào tên '{_enemyTrackName}' trong Timeline!");
                        return;
                }

// 4. Duyệt qua danh sách và gán binding
                foreach (var track in targetTracks)
                {
                        // Lệnh quan trọng: Gán Animator vào Track
                        _director.SetGenericBinding(track, enemyAnimator);
                }
              
                
                _director.playableAsset = timelineAsset;
                _director.time = 0;

                // 5. Sau khi gán xong xuôi thì mới chạy phim
                _director.Play();
        }

        public void PlayFinisherSuccess(Actor enemy, Action action =null)
        {
                PlayCinematic(finisherSuccess,enemy,action);
        }
        public void PlayFinisherFail(Actor enemy,Action action = null)
        {
                PlayCinematic(finisherFail,enemy,action);
        }


        private void OnDirectorStopped(PlayableDirector director)
        {
                // Khi phim dừng, kiểm tra xem có việc gì được dặn dò không
                if (_currentOnCompleteCallback != null)
                {
                        _currentOnCompleteCallback.Invoke(); // Thực hiện hành động!
                        _currentOnCompleteCallback = null;   // Xóa đi để không gọi nhầm lần sau
                }
        }

}
