<template>
    <div class="card">
        <div class="card-body">
            <h5 class="card-title">Change Session Information</h5>
            <div class="mb-3">
                <label for="sessionIdInput" class="form-label">Session ID</label>
                <input v-model="sessionId" class="form-control" id="sessionIdInput"
                    placeholder="Session ID">
            </div>
            <div class="mb-3">
                <label for="sessionNameInput" class="form-label">New Session Name</label>
                <input v-model="sessionName" class="form-control" id="sessionNameInput"
                    placeholder="Session Name">
            </div>
            <div class="mb-3">
                <label for="descriptionInput" class="form-label">Description</label>
                <textarea v-model="description" class="form-control" id="descriptionInput"
                    placeholder="Description" rows="3"></textarea>
            </div>
            <a href="#head" type="button" class="btn btn-primary" @click="createSession()">Create</a>
        </div>
    </div>
</template>

<script>
export default {
    data() {
        return {
            sessionName: '',
            description: '',
            sessionId: ''
        }
    },
    methods: {
        async createSession() {
            var data = {
                'sessionName': this.sessionName === '' ? null : this.sessionName,
                'description': this.description === '' ? null : this.sessionName
            }
            var response = await fetch(`${this.$root.apiUrl}/api/session/${this.sessionId}`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(data)
            });

            if (response.status === 200) {
                // redirect
                this.$parent.failed = false;
                this.$parent.message = `You have successfully changed information for session "${sessionId}".`;
            } else if (response.status === 404) {
                this.$parent.failed = true;
                this.$parent.message = `Invalid session ID.`;
            } else {
                this.$parent.failed = true;
                this.$parent.message = "Server error. Please try again later.";
            }
        }
    }
}
</script>
