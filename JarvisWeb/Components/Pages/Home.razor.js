


export function recordAudio() {
    if (window.isRecording) {
        window.isRecording = false;
        window.mediaRecorder.stop();
        return;
    }
    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
        console.log("getUserMedia supported.");
        navigator.mediaDevices
            .getUserMedia(
                // constraints - only audio needed for this app
                {
                    audio: true,
                },
            )

            // Success callback
            .then((stream) => {
                window.mediaRecorder = new MediaRecorder(stream);
                let chunks = [];

                window.mediaRecorder.ondataavailable = (e) => {
                    chunks.push(e.data);
                };
                window.mediaRecorder.onstop = (e) => {
                    const blob = new Blob(chunks, { type: "audio/ogg; codecs=opus" });
                    chunks = [];
                    const form = new FormData();
                    form.append('audio', blob);
                    fetch('/api/endOfDayNote/from-audio', {
                        method: 'POST',
                        body: form
                    }).then((r) => {
                        console.log('fetch result', r);
                    }).catch((exception) => {
                        console.log('fetch exception', ex);
                    })
                    stream.getTracks().forEach((track) => {
                        track.stop()
                    });
                }
                mediaRecorder.start()
                window.isRecording = true;
            })
            // Error callback
            .catch((err) => {
                console.error(`The following getUserMedia error occurred: ${err}`);
            });
    } else {
        console.log("getUserMedia not supported on your browser!");
    }
}

export function addHandlers() {
    console.log('I got loaded');
    const btn = document.getElementById('record-btn');
    if (!btn) return;
    btn.addEventListener('click', recordAudio)
}

export function beginAudioPlayBack() {
    let currentIndex = 0;
    let gapTime = 0;
    function checkAndPlayNext() {
        const currentAudio = document.getElementById(`audio-${currentIndex}`);
        if (!currentAudio) {
            gapTime++;
            if (gapTime > 30) {
                console.log("No more audio elements to play.");
                return;
            }
        }
        gapTime == 0;
        if (!currentAudio.paused) {
            setTimeout(checkAndPlayNext, 100);
        } else {
            currentIndex++;
            const nextAudio = document.getElementById(`audio-${currentIndex}`);
            if (nextAudio) {
                nextAudio.play().catch((err) => {
                    console.error(`Error playing audio-${currentIndex}:`, err);
                });
                setTimeout(checkAndPlayNext, 100);
            } else {
                console.log("Finished playing all audio elements.");
            }
        }
    }
    const currentAudio = document.getElementById(`audio-${currentIndex}`);
    currentAudio.play();
    setTimeout(checkAndPlayNext, 100);
}

