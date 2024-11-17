<template>
    <div class="card">
        <div class="card-body">
            <h5 class="card-title">Reload booklist</h5>
            <p> Reload booklist </p>
            <a href="#head" type="button" class="btn btn-primary" @click="reloadBooklist()">Reload</a>
        </div>
    </div>
</template>

<script>
export default {
    methods: {
        async reloadBooklist() {
            var response = await fetch(`${this.$root.apiUrl}/api/reload`, {
                method: "POST"
            });

            if (response.status == 200) {
                // redirect
                this.$parent.failed = false;
                this.$parent.message = "Booklist reloaded.";
            } else {
                this.$parent.failed = true;
                this.$parent.message = "Server error. Please try again later.";
            }
        }
    }
}
</script>
