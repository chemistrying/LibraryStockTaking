<template>
    <div class="card">
        <div class="card-body">
            <h5 class="card-title">Select Active Session</h5>
            <div class="mb-3">
                <label for="sessionIdInput" class="form-label">Session ID</label>
                <input v-model="sessionId" class="form-control" id="sessionIdInput"
                    placeholder="ID">
            </div>
            <a href="#head" type="button" class="btn btn-primary" @click="selectActiveSession()">Select</a>
        </div>
    </div>
</template>

<script>
export default {
    data() {
        return {
            sessionId: ''
        }
    },
    methods: {
        async selectActiveSession() {
            var response = await fetch(`${this.$root.apiUrl}/api/active/${this.sessionId}`, {
                method: "POST"
            });

            if (response.status == 200) {
                // redirect
                this.$parent.failed = false;
                this.$parent.message = "You have successfully changed the current active session.";
            } else if (response.status == 404) {
                this.$parent.failed = true;
                this.$parent.message = "Invalid session ID.";
            } else {
                this.$parent.failed = true;
                this.$parent.message = "Server error. Please try again later.";
            }
        }
    }
}
</script>
