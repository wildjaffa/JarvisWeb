


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
    btn.addEventListener('click', recordAudio)
}

