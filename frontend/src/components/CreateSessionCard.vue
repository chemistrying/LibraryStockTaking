<template>
    <div class="card">
        <div class="card-body">
            <h5 class="card-title">Create New Session</h5>
            <div class="mb-3">
                <label for="sessionNameInput" class="form-label">Session Name</label>
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
            description: ''
        }
    },
    methods: {
        async createSession() {
            var data = {
                'sessionName': this.sessionName,
                'description': this.description
            }
            var response = await fetch(`${this.$root.apiUrl}/api/session`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(data)
            });

            if (response.status == 201) {
                // redirect
                this.$parent.failed = false;
                var sessionId = response.json().id;
                this.$parent.message = `You have successfully created a session with id "${sessionId}".`;
            } else {
                this.$parent.failed = true;
                this.$parent.message = "Server error. Please try again later.";
            }
        }
    }
}
</script>
